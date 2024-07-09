import unittest
import game_manager
from Player import Player


class MyTestCase(unittest.TestCase):
    def test_game_order_3p(self):
        players = []
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        p2 = Player("socket1", "nome1", "id_lobby", "id1")
        p3 = Player("socket1", "nome1", "id_lobby", "id1")
        players.append(p1)
        players.append(p2)
        players.append(p3)

        game_manager._game_order(players)
        self.assertEqual(True, False)  # add assertion here

    def test_give_tank(self):
        print("TODO")
    def test_give_objective_cards(self):
        print("TODO")
    def test_give_territory_cards(self):
        print("TODO")
    def test_assign_default_tanks_to_territories(self):
        print("TODO")




if __name__ == '__main__':
    unittest.main()
