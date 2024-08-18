import random
import xml.etree.ElementTree as ET
from Card import Card
from Territory import Territory
import secrets
import numpy as np


def read_objects_cards():
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()
    cards = []

    for card in root.findall('objects/object'):
        card_id = card.get('id')
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text

        card = Card(card_id, image, function, description)
        cards.append(card)
    return cards


def read_territories_cards():
    tree = ET.parse('assets/config.xml')
    root = tree.getroot()
    cards = []

    for card in root.findall('.//territory'):
        card_id = card.get('id')
        name = card.find('name').text
        image = card.find('image').text
        function = card.find('function').text
        description = card.find('description').text
        continent = card.find('continent').text
        node = card.find('node').text

        card = Territory(card_id, image, function, description, name, continent, node, None, 1)
        cards.append(card)
    return cards


def generate_game_id():
    return ' '.join([str(random.randint(0, 999)).zfill(3) for _ in range(2)])


def generate_player_id():
    return secrets.token_hex(16)


def get_adj_matrix():
    return np.load('assets/adj_matrix.npy')


def get_neighbors_of(territory_node):
    adj_matrix = get_adj_matrix()
    neighbors = []
    row = adj_matrix[territory_node]
    for i, node in enumerate(row):
        if node == 1:
            neighbors.append(i)
    return neighbors
