class Player:
    def __init__(self, socket, name, lobby_id, player_id):
        self.name = name
        self.sock = socket
        self.lobby_id = lobby_id
        self.player_id = player_id # Unique id for player, to internal manage
        self.tanks_num = 0
        self.tanks_available = 0
        self.tanks_placed = 0
        self.objective_card = []
        self.territories = []

    def setObjectiveCard(self, goal_card):
        self.objective_card = goal_card

    def addTerritory(self, territory_card):
        self.territories.append(territory_card)

    def removeTerritory(self, territory_card):
        self.territories.remove(territory_card)

    def getTerritories(self):
        return self.territories

    def addTankToTerritory(self, territory_id):
        self.tanks_available -= 1
        self.tanks_placed += 1
        next((territory for territory in self.territories if territory_id == territory.id)).num_tanks += 1
