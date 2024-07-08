class Player:
    def __init__(self, socket, name, id_lobby):
        self.name = name
        self.sock = socket
        self.id_lobby = id_lobby
        self.tank_num = 0
        self.goal_card = None
        self.territory_card = None

    def setGoal_card(self, goal_card):
        self.goal_card = goal_card
        
    def addTerritory(self, territory_card):
        self.territory_card.append(territory_card)
    def removeTerritory(self, territory_card):
        self.territory_card.remove(territory_card)