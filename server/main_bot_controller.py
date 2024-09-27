from Client_manager import ClientManager
import utils
import asyncio
import random
import sys


async def game(client_manager, host_id, bot_name):
    print('Function game')
    while not client_manager.is_connected():
        await asyncio.sleep(1)
    print(f'SERVER: {client_manager.player.player_id}')
    client_manager.player.name = bot_name

    await client_manager.request_name_update_player_list()
    await client_manager.send_name()
    await client_manager.join_lobby_as_client(host_id)

    while client_manager.game_manager.get_extracted_number() == 0:
        await client_manager.request_name_update_player_list()
        await client_manager.send_name()
        await asyncio.sleep(1)
    extracted_number = client_manager.game_manager.get_extracted_number()
    print(f'EXTRACTED NUMBER: {extracted_number}')

    while client_manager.game_manager.get_game_order() == '':
        await asyncio.sleep(1)
    print(client_manager.game_manager.get_game_order())

    # Waiting for my turn
    while not client_manager.player.is_my_turn:
        await asyncio.sleep(1)

    # Choose army color
    client_manager.player.army_color = client_manager.game_manager.get_available_colors().pop()
    await client_manager.send_chosen_army_color()
    client_manager.player.is_my_turn = False
    print(f'COLOR CHOOSE: {client_manager.player.army_color}')

    # Waiting for objective card
    print('Waiting for card')
    while not client_manager.player.objective_card:
        await asyncio.sleep(1)
    print('Card received')

    # Start placing first tanks
    await setup(client_manager)
    await asyncio.sleep(1)

    # Start game and attack
    want_to_attack = 0.5
    want_to_move = 0.5
    first_round = True
    while True:
        if client_manager.player.is_my_turn:
            await asyncio.sleep(0.5)

            # REINFORCE PHASE
            if not first_round:
                await asyncio.sleep(2)
                print('Inizio fase di rinforzo')
                await reinforce_phase(client_manager)

            # ATTACK OR MOVE PHASE
            print('Inizio fase di attacco')
            if first_round:
                await asyncio.sleep(2)
            await choose_what_to_do(client_manager)

            print('Inizio fase di spostamenti')
            await strategic_move_phase(client_manager)

            client_manager.player.is_my_turn = False
            print('Passo il turno')
            first_round = False
        else:
            await asyncio.sleep(0.5)


async def strategic_move_phase(client_manager):
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)
    if objective_id == 1 or objective_id == 2:
        return
    # Check if I have isolated territories and move away its thanks
    isolate_territories = utils.get_isolate_territory(client_manager.player.territories,
                                                      client_manager.game_manager.all_territories)
    for isolate_territory in isolate_territories:
        if isolate_territory.num_tanks > 1:
            tanks_to_move = isolate_territory.num_tanks - 1
            friends = utils.get_friends_neighbors(isolate_territory, client_manager.player.territories,
                                                  client_manager.game_manager.all_territories)
            while tanks_to_move > 0:
                friend = random.choice(friends)
                friend.num_tanks += 1
                tanks_to_move -= 1
                isolate_territory.num_tanks -= 1
                print(f'Move thank from {isolate_territory.name} to {friend.name}')
    await client_manager.update_territories_state()
    while not client_manager.game_manager.all_territories:
        await asyncio.sleep(0.5)


async def choose_what_to_do(client_manager):
    # Function that understand where is better to move or attack based on objective
    all_territories = client_manager.game_manager.all_territories
    my_territories = client_manager.player.territories
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)
    my_strong_territories = list(
        filter(lambda terr: terr.num_tanks > 2, my_territories)
    )
    if objective_id == 1 or objective_id == 2:
        # Attack where is more simple
        terr_of_interest = list(
            filter(lambda terr: terr.player_id != client_manager.player.player_id, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 3:
        # Move forward North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'AF')
                                and terr not in my_territories, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 4:
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'OC')
                                and terr not in my_territories, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 5:
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'OC')
                                and terr not in my_territories, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 6:
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'AF')
                                and terr not in my_territories, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 7 or objective_id == 8:
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'EU' or terr.id[0:2] == 'SA' or terr.id[0:2] == 'OC')
                                and terr not in my_territories, all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 9:
        terr_of_interest = list(
            # == is wrong... Try client_manager.game_manager.get_player_color(terr.player_id).contains('red')
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'red', all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 10:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'blue', all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    elif objective_id == 11:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'green',
                   all_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    else:
        print('This objective is not supported.')



async def _reinforce(client_manager, terr_of_interest, my_territories, is_setup):
    try:
        tanks_available = client_manager.player.tanks_available
        if is_setup and tanks_available > 3:
            tanks_available = 3
            client_manager.player.tanks_available -= tanks_available
        elif is_setup and tanks_available <= 3:
            client_manager.player.tanks_available -= tanks_available

        # Reinforce territories forward enemy (not neighbor)
        await client_manager.request_shortest_path(my_territories, terr_of_interest)
        while not client_manager.game_manager.shortest_paths:
            await asyncio.sleep(0.5)
        paths = sorted(client_manager.game_manager.shortest_paths, key=len)
        for path in paths:
            index_of_terr_to_reinforce = 0
            for node in path:
                is_my_terr = list(
                    filter(lambda terr: terr.node == int(node), my_territories)
                )
                if is_my_terr:
                    index_of_terr_to_reinforce += 1
                else:
                    break
            terr_to_reinforce = list(
                filter(lambda terr: terr.node == path[index_of_terr_to_reinforce - 1], my_territories)
            ).pop()
            if tanks_available > 0:
                terr_to_reinforce.num_tanks += 1
                tanks_available -= 1
                print(f'Placed 1 tank in {terr_to_reinforce.name}')
        if not is_setup:
            client_manager.player.tanks_available = 0
    except Exception as e:
        client_manager.player.tanks_available = 0
        print(e)


async def reinforce_phase(client_manager):
    all_territories = client_manager.game_manager.all_territories
    my_territories = client_manager.player.territories
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)

    # Create a ranking of territories in danger and start from it
    # to reinforce
    if objective_id == 1 or objective_id == 2:
        terr_of_interest = list(
            filter(lambda terr: terr.player_id != client_manager.player.player_id, all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 3:
        # Place near North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'AF')
                                and terr not in my_territories, all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 4:
        # Place near North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'OC')
                                and terr not in my_territories, all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 5:
        # Place near North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'OC')
                                and terr not in my_territories, all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 6:
        # Place near North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'AF')
                                and terr not in my_territories, all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 7 or objective_id == 8:
        # Place near North America or Africa
        terr_of_interest = list(
            filter(lambda terr: (terr.id[0:2] == 'EU' or terr.id[0:2] == 'SA' or terr.id[0:2] == 'OC')
                                and terr not in my_territories,
                   all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 9:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'red',
                   all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 10:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'blue',
                   all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    elif objective_id == 11:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'green',
                   all_territories)
        )
        await _reinforce(client_manager, terr_of_interest, my_territories, False)

    await client_manager.update_territories_state()
    while not client_manager.game_manager.all_territories:
        await asyncio.sleep(0.5)
    client_manager.player.territories = list(
        filter(lambda terr: terr.player_id == client_manager.player.player_id,
               client_manager.game_manager.all_territories)
    )


async def setup(client_manager):
    while client_manager.player.tanks_available > 0:
        while not client_manager.player.is_my_turn:
            await asyncio.sleep(0.5)
        all_territories = client_manager.game_manager.all_territories
        my_territories = client_manager.player.territories
        objective = client_manager.player.objective_card
        objective_id = objective.id.replace('obj', '')
        objective_id = int(objective_id)
        if objective_id == 1 or objective_id == 2:
            # Conquered 8 or 24 territories
            terr_of_interest = list(
                filter(lambda terr: terr.player_id != client_manager.player.player_id, all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 3:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'AF')
                                    and terr not in my_territories, all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 4:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: (terr.id[0:2] == 'NA' or terr.id[0:2] == 'OC')
                                    and terr not in my_territories, all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 5:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'OC')
                                    and terr not in my_territories, all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 6:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: (terr.id[0:2] == 'AS' or terr.id[0:2] == 'AF')
                                    and terr not in my_territories, all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 7 or objective_id == 8:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: (terr.id[0:2] == 'EU' or terr.id[0:2] == 'SA' or terr.id[0:2] == 'OC')
                                    and terr not in my_territories,
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 9:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'red',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 10:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'blue',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)

        elif objective_id == 11:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'green',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories, True)
        await client_manager.update_territories_state()
        while not client_manager.game_manager.all_territories:
            await asyncio.sleep(0.5)
        client_manager.player.territories = list(
            filter(lambda terr: terr.player_id == client_manager.player.player_id,
                   client_manager.game_manager.all_territories)
        )
        client_manager.player.is_my_turn = False


async def _manage_attack(my_strong_territories, terr_of_interest, client_manager):
    await client_manager.request_shortest_path(my_strong_territories, terr_of_interest)
    while not client_manager.game_manager.shortest_paths:
        await asyncio.sleep(0.5)
    paths = sorted(client_manager.game_manager.shortest_paths, key=len)
    for path in paths:
        my_territories = list(filter(lambda terr: terr.player_id == client_manager.player.player_id,
                                     client_manager.game_manager.all_territories))
        win = False
        attacker = list(filter(lambda terr: terr.node == int(path[0]), my_territories)).pop()
        defender = list(filter(lambda terr: terr.node == int(path[1]), terr_of_interest))
        if defender:
            defender = defender.pop()
            win = await _attack(attacker, defender, client_manager)
        if win:
            await asyncio.sleep(2)
            terr_of_interest.remove(defender)


async def _attack(attacker, defender, client_manager):
    while attacker.num_tanks > 1 and attacker.num_tanks - defender.num_tanks > 0 and defender.player_id != attacker.player_id:
        tanks_attacker = attacker.num_tanks - 1
        if tanks_attacker > 3:
            tanks_attacker = 3
        client_manager.game_manager.set_im_attacking(True)
        await client_manager.attack_enemy_territory(attacker, defender, tanks_attacker)
        print(f'{attacker.name} is attacking {defender.name} with {tanks_attacker} tanks')
        while not client_manager.game_manager.extracted_my_numbers:
            await asyncio.sleep(0.5)
        print(f'MY NUMBERS: {client_manager.game_manager.extracted_my_numbers}')
        print(f'ENEMY NUMBERS: {client_manager.game_manager.extracted_enemy_numbers}')
        while not client_manager.game_manager.all_territories:
            await asyncio.sleep(0.5)
        client_manager.game_manager.reset_enemy_extracted_numbers()
        client_manager.game_manager.reset_my_extracted_numbers()
        defender = list(
            filter(lambda terr: terr.node == defender.node, client_manager.game_manager.all_territories)
        ).pop()
        attacker = list(
            filter(lambda terr: terr.node == attacker.node, client_manager.game_manager.all_territories)
        ).pop()
        print(f'{defender.name} is own by {defender.player_id}')
    return defender.player_id == attacker.player_id


async def main(game_id, bot_name):
    print('Client started!')
    client_manager = ClientManager()
    await asyncio.gather(client_manager.start_client('localhost'), game(client_manager, game_id, bot_name))


if __name__ == '__main__':
    game_id = sys.argv[1].replace('_', ' ')
    bot_name = sys.argv[2]
    asyncio.run(main(game_id, bot_name))