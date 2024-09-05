class Card:

    def __init__(self, card_id, image, function, description, player_id=None):
        self.id = card_id
        self.image = image
        self.function = function
        self.description = description
        self.player_id = player_id

    def to_dict(self):
        return {
            "id": self.id,
            "image": self.image,
            "function": self.function,
            "description": self.description,
            "player_id": self.player_id
        }

    @classmethod
    def from_dict(cls, data):
        return cls(data["id"], data["image"], data["function"], data["description"], data["player_id"])

    def __repr__(self):
        return (f"Card(id={self.id}, image={self.image}, function={self.function}, "
                f"description={self.description}, player_id={self.player_id})")

    def __eq__(self, other):
        return self.id == other.id

    def __hash__(self):
        return hash(self.id)
