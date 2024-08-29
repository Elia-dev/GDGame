from bot.Client_manager import ClientManager
import utils
import asyncio
import random

from Territory import Territory


async def game(client_manager):
    print('Function game')
    while not client_manager.is_connected():
        await asyncio.sleep(1)
    print(f'SERVER: {client_manager.player.player_id}')
    client_manager.player.name = 'PythonSgravato'

    await client_manager.request_name_update_player_list()
    await client_manager.send_name()
    await client_manager.join_lobby_as_client('572 853')

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
    await reinforce_phase(client_manager, True)

    # Start game and attack
    while True:
        if client_manager.player.is_my_turn:
            await asyncio.sleep(1)
            # REINFORCE PHASE
            if client_manager.player.tanks_available > 0:
                await reinforce_phase(client_manager, False)

            # ATTACK PHASE
            terr_attackers = list(
                filter(lambda terr: terr.num_tanks > 1, client_manager.player.territories)
            )
            if terr_attackers:
                terr_attacker = random.choice(terr_attackers)
                territories_to_attack = utils.get_neighbors_of(terr_attacker, client_manager.game_manager.all_territories)
                # Ensure that neighbors is not mine
                territories_to_attack = list(
                    filter(lambda terr: terr not in client_manager.player.territories, territories_to_attack)
                )
                terr_to_attack = territories_to_attack.pop()
                tanks_attacker = terr_attacker.num_tanks - 1
                if tanks_attacker > 3:
                    tanks_attacker = 3
                print(f'{terr_attacker} is attacking {terr_to_attack} with {tanks_attacker} tanks')

                client_manager.attack_enemy_territory(terr_attacker, terr_to_attack, tanks_attacker)
        else:
            await asyncio.sleep(1)


    print('DEAD')


async def reinforce_phase(client_manager, setup):
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
            if setup:
                await client_manager.update_territories_state()
                client_manager.player.is_my_turn = False
        else:
            await asyncio.sleep(1)

    if not setup:
        await client_manager.update_territories_state()


async def main():
    print('Client started!')
    client_manager = ClientManager()
    await asyncio.gather(client_manager.start_client(), game(client_manager))


if __name__ == '__main__':
    asyncio.run(main())
