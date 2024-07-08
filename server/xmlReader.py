import xml.etree.ElementTree as ET

def readCards():
    # Carica il file XML
    tree = ET.parse('config.xml')
    root = tree.getroot()

    # Lista per salvare le carte
    cards = []

    # Itera su tutti i nodi <card> nel file XML
    for card in root.findall('Card'):
        image_path = card.find('Image').text
        function = card.find('Function').text
        description = card.find('Description').text

    # Crea un dizionario per ogni carta e aggiungilo alla lista
    cards.append({'image_path': image_path, 'function': function, 'description': description})
    # Stampa la lista di carte

#for card in cards:
    #print(card)




