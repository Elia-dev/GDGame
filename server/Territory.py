from Card import Card

class Territory(Card):
    def __init__(self, card_id, image, function, description, name, continent, node, player_id=None, num_tanks=1):
        super().__init__(card_id, image, function, description, player_id)
        self.name = name
        self.num_tanks = num_tanks
        self.continent = continent
        self.node = node

    def to_dict(self):
        data = super().to_dict()
        data.update({
            "name": self.name,
            "num_tanks": self.num_tanks,
            "continent": self.continent,
            "node": self.node
        })
        return data

    @classmethod
    def from_dict(cls, data):
        return cls(data["id"], data["image"], data["function"], data["description"],
                   data["name"], data["continent"], data["node"], data["player_id"], data["num_tanks"])

    def __repr__(self):
        return (
            f"Territory(id={self.id}, image={self.image}, function={self.function}, description={self.description}, "
            f"player_id={self.player_id}, name={self.name}, num_tanks={self.num_tanks}, continent={self.continent}, node={self.node})")

    def __eq__(self, other):
        if not isinstance(other, Territory):
            return False
        return (super().__eq__(other) and
                self.name == other.name and
                self.continent == other.continent and
                self.node == other.node)

    def __hash__(self):
        # Ensure the class is hashable if you need to use it in sets or as dictionary keys
        return hash((super.__hash__(self), self.name, self.continent, self.node))
