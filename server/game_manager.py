import random

import connection_manager
from Player import Player
import utils

goals = ["Conquistare 18 territori presidiandoli con almeno due armate ciascuno"]


def _tank_start_num(num_player):
    switcher = {
        3: 35,
        4: 30,
        5: 25,
        6: 20
    }
    return switcher.get(num_player)


def _game_order(players):
    response = {}
    for player in players:
        #player.send(f"Press any key to throw a gaming dice!".encode("utf-8"))
        #player.recv(1024).decode("utf-8")
        gaming_dice = random.randint(1, 6)
        response[player] = gaming_dice
        #player.send(f"You got {gaming_dice}\n".encode("utf-8"))

    sorted_player = [item[0] for item in sorted(response.items(), key=lambda item: item[1])]
    sorted_player.reverse()

    # Create player object and notify its position
    players = []
    for i, player in enumerate(sorted_player):
        #player.send(f"You are the {i + 1}° player".encode("utf-8"))
        players.append(Player(player))

    return players


def _give_tank(players):
    num_tanks = _tank_start_num(len(players))
    for player in players:
        player.tanks_num = num_tanks
        player.tanks_available = num_tanks


def _give_objective_cards(players):
    cards = utils.read_objects_cards()
    for player in players:
        card_drawn = cards[random.randint(0, len(cards) - 1)]
        card_drawn.player_id = player.player_id
        player.objective_card = card_drawn
        cards.remove(card_drawn)
        #player.sock.send(f"GOAL:\n{card_drawn.description}".encode("utf-8"))


def _give_territory_cards(players):
    cards = utils.read_territories_cards()
    i = 0
    while cards:
        for player in players:
            if (cards):
                card_drawn = cards[random.randint(0, len(cards) - 1)]
                card_drawn.player_id = player.player_id
                player.addTerritory(card_drawn)
                cards.remove(card_drawn)
                #player.sock.send(f"Territory extracted:\n{card_drawn.description}".encode("utf-8"))


def _assign_default_tanks_to_territories(players):
    for player in players:
        territories = player.getTerritories()
        for territory in territories:
            territory.num_tanks = 1
        player.tanks_placed = player.tanks_num - len(player.territories)
        player.tanks_available = player.tanks_num - player.tanks_placed


def game_main(players, host_id):
    # Define the game order
    for player in players:
        player.send(f"Game {host_id} started!".encode("utf-8"))
    players = _game_order(players)

    # Give tank
    _give_tank(players)
    connection_manager.update_state(players)
    # Give objective card
    _give_objective_cards(players)
    connection_manager.update_state(players)
    # Give territory card
    _give_territory_cards(players)
    connection_manager.update_state(players)
    # Give 1 tank for each player's territory
    _assign_default_tanks_to_territories(players)
    connection_manager.update_state(players)

    # First tank assignment by players
    '''S = server, C = client, P = problema'''
    ''' 
    S: ripeto l'assegnazione finché ogni player non ha piazzato tutti i suoi tank
        S: assegno token al primo giocatore, il token servirà solo come aggancio tra client e server
            il server sa a chi sta il turno, ma il client non sa se sta a lui
            riesce a capirlo in base a se possiede il token o no
           C: attendo di avere il token, appena lo ricevo sblocco la UI
           P: come codifico il concetto di token?
           C: piazzo 3 tank dove voglio nei miei territori
           P: come faccio a far piazzare esattamente 3 tank al player?
           C: appena piazzati i tank passo il turno e perdo il token
        S: assegno il token al secondo giocatore e ripeto       
    '''

    # Main loop




