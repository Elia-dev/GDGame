from Card import Card


class Territory(Card):
    def __init__(self, card_id, image, function, description, player_id, name, num_tanks, continent):
        super().__init__(card_id, image, function, description, player_id)
        self.name = name
        self.num_tanks = num_tanks
        self.continent = continent

    def to_dict(self):
        data = super().to_dict()
        data.update({
            "name": self.name,
            "num_tanks": self.num_tanks,
            "continent": self.continent,
        })
        return data

    def __repr__(self):
        return (
            f"Territory(id={self.id}, image={self.image}, function={self.function}, description={self.description}, "
            f"player_id={self.player_id}, name={self.name}, num_tanks={self.num_tanks}, continent={self.continent})")
