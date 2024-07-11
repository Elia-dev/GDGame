from Card import Card

class Territory(Card):
    def __init__(self, card_id, image, function, description, player_id, name, num_tanks, continent):
        super().__init__(card_id, image, function, description, player_id)
        self.name = name
        self.num_tanks = num_tanks
        self.continent = continent


