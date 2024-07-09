class Player:
    def __init__(self, socket, name, id_lobby, id_player):
        self.name = name
        self.sock = socket
        self.id_lobby = id_lobby
        self.id_player = id_player # Unique id for player, to internal manage
        self.tank_num = 0
        self.goal_card = None
        self.territories = None

    def setGoal_card(self, goal_card):
        self.goal_card = goal_card

    def addTerritory(self, territory_card):
        self.territories.append(territory_card)
    def removeTerritory(self, territory_card):
        self.territories.remove(territory_card)

    def getTerritories(self):
        return self.territories
