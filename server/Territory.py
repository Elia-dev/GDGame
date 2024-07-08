from Card import Card

class Territory(Card):
    def __init__(self, card_id, image, function, description, name, num_armies, continent):
        super().__init__(card_id, image, function, description)
        self.name = name
        self.num_armies = num_armies
        self.continent = continent

        ##Come associare il territorio a chi l'ha conquistato?
        ##Il nome potrebbe essere duplicato, serve un id nella classe Player, magari lo stesso che ha il server per identificare i giocatori

