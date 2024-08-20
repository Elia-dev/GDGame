from Card import Card
from Objective import Objective
from Territory import Territory


class Player:
    def __init__(self, websocket, name=None, lobby_id=None, player_id=None):
        self.name = name
        self.sock = websocket
        self.lobby_id = lobby_id
        self.player_id = player_id  # Unique id for player, to internal manage
        self.tanks_num = 0
        self.tanks_available = 0
        self.tanks_placed = 0
        self.objective_card = None
        self.territories = []
        self.army_color = None
        self.killed_by = None

    def to_dict(self):
        return {
            "name": self.name,
            "sock": "sock",  # How to rapresent socket ?
            "lobby_id": self.lobby_id,
            "player_id": self.player_id,
            "tanks_num": self.tanks_num,
            "tanks_available": self.tanks_available,
            "tanks_placed": self.tanks_placed,
            "objective_card": self.objective_card.to_dict(),
            "territories": [territory.to_dict() for territory in self.territories]
        }

    def __repr__(self):
        return (f"Player(name={self.name}, socket={self.sock}, lobby_id={self.lobby_id}, player_id={self.player_id}, "
                f"tanks_num={self.tanks_num}, tanks_available={self.tanks_available}, tanks_placed={self.tanks_placed}, "
                f"objective_card={self.objective_card}, territories={self.territories})")

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

    @classmethod
    def from_dict(cls, data):
        player = cls(data["sock"], data["name"], data["lobby_id"], data["player_id"])
        player.tanks_num = data["tanks_num"]
        player.tanks_available = data["tanks_available"]
        player.tanks_placed = data["tanks_placed"]
        player.objective_card = Objective.from_dict(data["objective_card"])
        player.territories = [Territory.from_dict(territory_data) for territory_data in data["territories"]]
        return player
