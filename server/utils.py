import random
import os
import heapq
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
        node = int(card.find('node').text)

        card = Territory(card_id, image, function, description, name, continent, node, None, 1)
        cards.append(card)
    return cards


def generate_game_id():
    return ' '.join([str(random.randint(0, 999)).zfill(3) for _ in range(2)])


def generate_player_id():
    return secrets.token_hex(16)


def get_adj_matrix(path):
    return np.load(path)


def get_neighbors_node_of(territory_node, file_path):
    adj_matrix = get_adj_matrix(file_path)
    neighbors = []
    row = adj_matrix[territory_node]
    for i, node in enumerate(row):
        if node == 1:
            neighbors.append(i)
    return neighbors


def get_territory_from_node(node, all_territories):
    for territory in all_territories:
        if node == territory.node:
            return territory
    return None


def get_neighbors_of(territory, all_terr):
    territories = []
    nodes = get_neighbors_node_of(territory.node, os.path.join(os.getcwd(), 'assets/adj_matrix.npy'))
    for node in nodes:
        territories.append(get_territory_from_node(node, all_terr))
    return territories


def get_isolate_territory(my_territories, all_territories):
    isolate_territories = []
    for terr in my_territories:
        enemies = get_enemy_neighbors_of(terr, my_territories, all_territories)
        if not enemies:
            isolate_territories.append(terr)
    return isolate_territories


def get_friends_neighbors(territory, my_territories, all_terr):
    friends = []
    neighbors = get_neighbors_of(territory, all_terr)
    for neighbor in neighbors:
        for my_terr in my_territories:
            if neighbor.id == my_terr.id:
                friends.append(neighbor)
    return friends


def get_enemy_neighbors_of(territory, my_terrs, all_terrs):
    neighbors = get_neighbors_of(territory, all_terrs)
    enemy = list(
        filter(lambda neighbor: neighbor not in my_terrs, neighbors)
    )
    return enemy


def get_all_enemies_neighbors_of(my_terrs, all_terrs):
    enemy = []
    neighbors = []
    for my_terr in my_terrs:
        neighbors += get_neighbors_of(my_terr, all_terrs)
    for neighbor in neighbors:
        for my_terr in my_terrs:
            if neighbor.id != my_terr.id:
                enemy.append(neighbor)
    return enemy


def update_adjacent_matrix(players, adj_matrix):
    for player in players:
        for territory in player.territories:
            node = territory.node
            neighbors_node = get_neighbors_node_of(node, os.path.join(os.getcwd(), 'assets/adj_matrix.npy'))
            for neighbor in neighbors_node:
                adj_matrix[neighbor][node] = territory.num_tanks
    print(f'Adjacent matrix updated')
    return adj_matrix


def get_shortest_path(from_node, to_node, adj_matrix):
    # Number of nodes in the graph
    num_nodes = adj_matrix.shape[0]

    # Distance array - Initialize all distances to infinity
    distances = [float('inf')] * num_nodes
    # Previous node array - To reconstruct the path
    previous_nodes = [None] * num_nodes

    # Distance to the start node is 0
    distances[from_node] = 0

    # Priority queue to explore the nodes with the smallest distance
    priority_queue = [(0, from_node)]

    while priority_queue:
        # Get the node with the smallest distance
        current_distance, current_node = heapq.heappop(priority_queue)

        # If we have already reached the target node, we can stop
        if current_node == to_node:
            break

        # Explore neighbors
        for neighbor in range(num_nodes):
            if adj_matrix[current_node][neighbor] != 0:
                distance = adj_matrix[current_node][neighbor]
                new_distance = current_distance + distance

                # If a shorter path is found
                if new_distance < distances[neighbor]:
                    distances[neighbor] = new_distance
                    previous_nodes[neighbor] = current_node
                    heapq.heappush(priority_queue, (new_distance, neighbor))

    # Reconstruct the path
    path = []
    current_node = to_node
    while current_node is not None:
        path.append(current_node)
        current_node = previous_nodes[current_node]

    # If the path is not empty and the last node is the start node, return the path
    if path[-1] == from_node:
        path.reverse()
        return path
    else:
        # If there is no path from from_node to to_node
        return []

