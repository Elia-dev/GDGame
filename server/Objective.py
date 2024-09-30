from Card import Card

class Objective(Card):
    def __init__(self, card_id, image, function, description, player_id=None):
        super().__init__(card_id, image, function, description, player_id)

    def to_dict(self):
        data = super().to_dict()
        return data

    @classmethod
    def from_dict(cls, data):
        return cls(data["id"], data["image"], data["function"], data["description"], data["player_id"])
