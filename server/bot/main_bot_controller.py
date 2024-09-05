from bot.Client_manager import ClientManager
import utils
import asyncio
import random

from Territory import Territory


async def game(client_manager, host_id):
    print('Function game')
    while not client_manager.is_connected():
        await asyncio.sleep(1)
    print(f'SERVER: {client_manager.player.player_id}')
    client_manager.player.name = 'Python'

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

    # Start game and attack
    want_to_attack = 0.5
    want_to_move = 0.5
    first_round = True
    while True:
        if client_manager.player.is_my_turn:
            await asyncio.sleep(1)

            # REINFORCE PHASE
            if client_manager.player.tanks_available > 0 and not first_round:
                await reinforce_phase(client_manager)
            await asyncio.sleep(1)

            # ATTACK OR MOVE PHASE
            await choose_what_to_do(client_manager)
            await strategic_move_phase(client_manager)

            await client_manager.update_territories_state()
            client_manager.player.is_my_turn = False
            print('Passo il turno')
            first_round = False
        else:
            await asyncio.sleep(1)

    print('DEAD')


async def strategic_move_phase(client_manager):
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)
    if objective_id == 1 or objective_id == 2:
        return
    # Check if I have isolated territories and move away its thanks
    isolate_terr = utils.get_isolate_territory(client_manager.player.territories, client_manager.game_manager.all_territories)
    attempt = 0
    while isolate_terr and attempt < 5:
        thanks_to_move = isolate_terr.num_thank - 1
        friends = utils.get_friends_neighbors(isolate_terr, client_manager.player.territories, client_manager.game_manager.all_territories)
        while thanks_to_move > 0:
            friend = random.choice(friends)
            friend.num_thank += 1
            thanks_to_move -= 1
            isolate_terr.num_thank -= 1
            print(f'Move thank from {isolate_terr.name} to {friend.name}')
        attempt += 1


async def choose_what_to_do(client_manager):
    # Function that understand where is better to move or attack based on objective
    all_territories = client_manager.game_manager.all_territories
    my_territories = client_manager.player.territories
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)
    if objective_id == 1 or objective_id == 2:
        # Attack where is more simple
        for territory in my_territories:
            enemies = utils.get_enemy_neighbors_of(territory, my_territories, all_territories)
            for enemy in enemies:
                await _attack(territory, enemy, client_manager)

    if objective_id == 3:
        # Move forward North America or Africa
        terr_of_interest = list(
            filter(lambda terr: terr.id[0:2] == 'NA' or terr.id[0:2] == 'AF', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 4:
        terr_of_interest = list(
            filter(lambda terr: terr.id[0:2] == 'NA' or terr.id[0:2] == 'OC', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 5:
        terr_of_interest = list(
            filter(lambda terr: terr.id[0:2] == 'AS' or terr.id[0:2] == 'OC', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 6:
        terr_of_interest = list(
            filter(lambda terr: terr.id[0:2] == 'AS' or terr.id[0:2] == 'AF', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 7 or objective_id == 8:
        terr_of_interest = list(
            filter(lambda terr: terr.id[0:2] == 'EU' or terr.id[0:2] == 'SA' or terr.id[0:2] == 'OC',
                   all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 9:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'red', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 10:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'blue', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)

    if objective_id == 11:
        terr_of_interest = list(
            filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'green', all_territories)
        )
        my_strong_territories = list(
            filter(lambda terr: terr.num_tanks > 2, my_territories)
        )
        if not my_strong_territories:
            return
        await _manage_attack(my_strong_territories, terr_of_interest, client_manager)


async def _reinforce(client_manager, terr_of_interest, my_territories):
    paths = []
    for my_terr in my_territories:
        for enemy in terr_of_interest:
            await client_manager.request_shortest_path(my_terr.node, enemy.node)
            while not client_manager.game_manager.shortest_path:
                await asyncio.sleep(1)
            paths.append(client_manager.game_manager.shortest_path)
            client_manager.game_manager.shortest_path = []
    paths = sorted(paths, key=len)
    while client_manager.player.tanks_available > 0:
        for path in paths:
            my_territory = path[0]
            enemy = path[1]
            if client_manager.player.tanks_available > (
                    enemy.num_tank - 1) and my_territory.num_tanks <= enemy.num_tanks:
                my_territory.num_tanks += enemy.num_tanks
                client_manager.player.tanks_available -= enemy.num_tanks
            elif my_territory.num_tanks <= enemy.num_tanks:
                my_territory.num_tanks = client_manager.player.tanks_available
                client_manager.player.tanks_available = 0
            else:
                # Territory already strong
                pass


async def reinforce_phase(client_manager):
    all_territories = client_manager.game_manager.all_territories
    my_territories = client_manager.player.territories
    objective = client_manager.player.objective_card
    objective_id = objective.id.replace('obj', '')
    objective_id = int(objective_id)

    if client_manager.player.is_my_turn:
        if objective_id == 1 or objective_id == 2:
            necessary_tanks = {}
            for my_territory in my_territories:
                enemies = utils.get_enemy_neighbors_of(my_territory, my_territories, all_territories)
                enemies_tanks = 0
                for enemy in enemies:
                    enemies_tanks += enemy.num_tanks
                necessary_tanks[my_territory.id] = enemies_tanks - my_territory.num_tanks

            necessary_tanks = dict(sorted(necessary_tanks.items(), key=lambda item: item[1], reverse=True))
            while client_manager.player.tanks_available > 0:
                for terr, tanks in necessary_tanks.items():
                    if client_manager.player.tanks_available > (tanks - 1):
                        terr.num_tanks += tanks
                        client_manager.player.tanks_available -= tanks
                    else:
                        terr.num_tanks += client_manager.player.tanks_available
                        client_manager.player.tanks_available = 0

        if objective_id == 3:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: terr.id[0:2] == 'NA' or terr.id[0:2] == 'AF', all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 4:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: terr.id[0:2] == 'NA' or terr.id[0:2] == 'OC', all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 5:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: terr.id[0:2] == 'AS' or terr.id[0:2] == 'OC', all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 6:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: terr.id[0:2] == 'AS' or terr.id[0:2] == 'AF', all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 7 or objective_id == 8:
            # Place near North America or Africa
            terr_of_interest = list(
                filter(lambda terr: terr.id[0:2] == 'EU' or terr.id[0:2] == 'SA' or terr.id[0:2] == 'OC',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 9:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'red',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 10:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'blue',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        if objective_id == 11:
            terr_of_interest = list(
                filter(lambda terr: client_manager.game_manager.get_player_color(terr.player_id) == 'green',
                       all_territories)
            )
            await _reinforce(client_manager, terr_of_interest, my_territories)

        await client_manager.update_territories_state()
    else:
        await asyncio.sleep(1)


async def setup(client_manager):
    territories_len = len(client_manager.player.territories)
    while client_manager.player.tanks_available > 0:
        print('Wait to place tanks')
        if client_manager.player.is_my_turn:
            territory = client_manager.player.territories[random.randint(0, territories_len - 1)]
            if client_manager.player.tanks_available > 3:
                tank_to_place = 3
            else:
                tank_to_place = client_manager.player.tanks_available
            territory.num_tanks += tank_to_place
            client_manager.player.tanks_available -= tank_to_place
            client_manager.player.tanks_placed += tank_to_place
            print(f"Placed {tank_to_place} tanks in {territory.name}")
            await client_manager.update_territories_state()
            client_manager.player.is_my_turn = False
        else:
            await asyncio.sleep(1)


async def _manage_attack(my_strong_territories, terr_of_interest, client_manager):
    my_territories = client_manager.player.territories
    paths = []
    for my_terr in my_strong_territories:
        for enemy in terr_of_interest:
            await client_manager.request_shortest_path(my_terr.node, enemy.node)
            while not client_manager.game_manager.shortest_path:
                await asyncio.sleep(1)
            paths.append(client_manager.game_manager.shortest_path)
            client_manager.game_manager.shortest_path = []
    paths = sorted(paths, key=len)
    for path in paths:
        attacker = list(filter(lambda terr: terr.node == path[0], my_territories)).pop()
        defender = list(filter(lambda terr: terr.node == path[1], terr_of_interest)).pop()
        await _attack(attacker, defender, client_manager)


async def _attack(attacker, defender, client_manager):
    while attacker.num_tanks > 1 and attacker.num_tanks - defender.num_tanks > 0 and defender.player_id != attacker.player_id:
        tanks_attacker = attacker.num_tanks - 1
        if tanks_attacker > 3:
            tanks_attacker = 3
        client_manager.game_manager.set_im_attacking(True)
        await client_manager.attack_enemy_territory(attacker, defender, tanks_attacker)
        print(f'{attacker.name} is attacking {defender.name} with {tanks_attacker} tanks')
        await asyncio.sleep(2)
        while client_manager.game_manager.get_im_attacking():
            await asyncio.sleep(1)
        await asyncio.sleep(2)


async def attack_phase(client_manager):
    terr_attackers = list(
        filter(lambda terr: terr.num_tanks > 1, client_manager.player.territories)
    )
    if terr_attackers:
        terr_attacker = random.choice(terr_attackers)
        territories_to_attack = utils.get_enemy_neighbors_of(terr_attacker, client_manager.player.territories, client_manager.game_manager.all_territories)
        if territories_to_attack:
            terr_to_attack = random.choice(territories_to_attack)
            tanks_attacker = terr_attacker.num_tanks - 1
            if tanks_attacker > 3:
                tanks_attacker = 3
            print(f'{terr_attacker} is attacking {terr_to_attack} with {tanks_attacker} tanks')
            await client_manager.attack_enemy_territory(terr_attacker, terr_to_attack, tanks_attacker)


async def main():
    print('Client started!')
    client_manager = ClientManager()
    await asyncio.gather(client_manager.start_client('150.217.51.105'), game(client_manager, '149 489'))


if __name__ == '__main__':
    asyncio.run(main())
