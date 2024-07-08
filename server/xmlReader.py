import xml.etree.ElementTree as ET
from card import Card


def read_goal_cards():
    # Carica il file XML
    tree = ET.parse('asset/config.xml')
    root = tree.getroot()

    # Lista per salvare le carte
    cards = []

    # Itera su tutti i nodi <card> nel file XML
    for card in root.findall('Card'):
        card_obj = Card(1,
                        card.find('Image').text,
                        card.find('Function').text,
                        card.find('Description').text)
        print(card.find('Image').text)
        cards.append(card_obj)

    return cards

