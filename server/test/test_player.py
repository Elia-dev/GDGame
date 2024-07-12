import unittest

from Player import Player
from Territory import Territory


class MyTestCase(unittest.TestCase):
    def test_addTankToTerritory(self):
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        t1 = Territory("01", "image", "function", "description",
                       "id1", "name", 0, "continent")

        p1.tanks_num = 1
        p1.tanks_placed = 0
        p1.tanks_available = 1
        p1.addTerritory(t1)
        p1.addTankToTerritory(t1.id)
        self.assertEqual(p1.tanks_num, 1, "Error adding tank to territory")
        self.assertEqual(p1.tanks_placed, 1, "Error adding tank to territory")
        self.assertEqual(p1.tanks_available, 0, "Error adding tank to territory")
        self.assertEqual(p1.territories[0].num_tanks, 1, "Error adding tank to territory")


if __name__ == '__main__':
    unittest.main()
