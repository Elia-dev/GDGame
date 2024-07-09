from Card import Card

class Territory(Card):
    def __init__(self, card_id, image, function, description, name, num_tanks, continent):
        super().__init__(card_id, image, function, description)
        self.name = name
        self.num_armies = num_tanks
        self.continent = continent


