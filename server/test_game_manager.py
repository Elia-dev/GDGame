import unittest
import game_manager
import utils
from Player import Player


class MyTestCase(unittest.TestCase):
    def __init_players_3p(self):
        players = []
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        p2 = Player("socket2", "nome2", "id_lobby", "id2")
        p3 = Player("socket3", "nome3", "id_lobby", "id3")
        players.append(p1)
        players.append(p2)
        players.append(p3)
        return players

    def __init_players_4p(self):
        players = []
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        p2 = Player("socket2", "nome2", "id_lobby", "id2")
        p3 = Player("socket3", "nome3", "id_lobby", "id3")
        p4 = Player("socket4", "nome4", "id_lobby", "id4")
        players.append(p1)
        players.append(p2)
        players.append(p3)
        players.append(p4)
        return players

    def __init_players_5p(self):
        players = []
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        p2 = Player("socket2", "nome2", "id_lobby", "id2")
        p3 = Player("socket3", "nome3", "id_lobby", "id3")
        p4 = Player("socket4", "nome4", "id_lobby", "id4")
        p5 = Player("socket5", "nome5", "id_lobby", "id5")
        players.append(p1)
        players.append(p2)
        players.append(p3)
        players.append(p4)
        players.append(p5)
        return players

    def __init_players_6p(self):
        players = []
        p1 = Player("socket1", "nome1", "id_lobby", "id1")
        p2 = Player("socket2", "nome2", "id_lobby", "id2")
        p3 = Player("socket3", "nome3", "id_lobby", "id3")
        p4 = Player("socket4", "nome4", "id_lobby", "id4")
        p5 = Player("socket5", "nome5", "id_lobby", "id5")
        p6 = Player("socket6", "nome6", "id_lobby", "id6")
        players.append(p1)
        players.append(p2)
        players.append(p3)
        players.append(p4)
        players.append(p5)
        players.append(p6)
        return players

    def test_num_players_game_order_3p(self):
        players = self.__init_players_3p()
        game_manager._game_order(players)
        self.assertEqual(len(players), 3, "Wrong player's number")

    def test_num_players_game_order_4p(self):
        players = self.__init_players_4p()
        game_manager._game_order(players)
        self.assertEqual(len(players), 4, "Wrong player's number")

    def test_num_players_game_order_5p(self):
        players = self.__init_players_5p()
        game_manager._game_order(players)
        self.assertEqual(len(players), 5, "Wrong player's number")

    def test_num_players_game_order_6p(self):
        players = self.__init_players_6p()
        game_manager._game_order(players)
        self.assertEqual(len(players), 6, "Wrong player's number")

    def test_give_tank_3p(self):
        players = self.__init_players_3p()
        game_manager._give_tank(players)
        for player in players:
            self.assertEqual(player.tanks_num, 35,
                             "Incorrect tank number assignment")
            self.assertEqual(player.tanks_available, 35,
                             "Incorrect tank number assignment")

    def test_give_tank_4p(self):
        players = self.__init_players_4p()
        game_manager._give_tank(players)
        for player in players:
            self.assertEqual(player.tanks_num, 30,
                             "Incorrect tank number assignment")
            self.assertEqual(player.tanks_available, 30,
                             "Incorrect tank number assignment")

    def test_give_tank_5p(self):
        players = self.__init_players_5p()
        game_manager._give_tank(players)
        for player in players:
            self.assertEqual(player.tanks_num, 25,
                             "Incorrect tank number assignment")
            self.assertEqual(player.tanks_available, 25,
                             "Incorrect tank number assignment")

    def test_give_tank_6p(self):
        players = self.__init_players_6p()
        game_manager._give_tank(players)
        for player in players:
            self.assertEqual(player.tanks_num, 20,
                             "Incorrect tank number assignment")
            self.assertEqual(player.tanks_available, 20,
                             "Incorrect tank number assignment")

    def test_give_objective_cards_3p(self):
        players = self.__init_players_3p()
        game_manager._give_objective_cards(players)
        for player in players:
            self.assertIsNotNone(player.objective_card,
                                 "Error during objective assignment")
            self.assertEqual(player.objective_card.player_id, player.player_id, "Error during objective assignment")

    def test_give_objective_cards_4p(self):
        players = self.__init_players_4p()
        game_manager._give_objective_cards(players)
        for player in players:
            self.assertIsNotNone(player.objective_card,
                                 "Error during objective assignment")
            self.assertEqual(player.objective_card.player_id, player.player_id, "Error during objective assignment")

    def test_give_objective_cards_5p(self):
        players = self.__init_players_5p()
        game_manager._give_objective_cards(players)
        for player in players:
            self.assertIsNotNone(player.objective_card,
                                 "Error during objective assignment")
            self.assertEqual(player.objective_card.player_id, player.player_id, "Error during objective assignment")

    def test_give_objective_cards_6p(self):
        players = self.__init_players_6p()
        game_manager._give_objective_cards(players)
        for player in players:
            self.assertIsNotNone(player.objective_card,
                                 "Error during objective assignment")
            self.assertEqual(player.objective_card.player_id, player.player_id, "Error during objective assignment")

    def test_give_objective_cards_no_duplicates_3p(self):
        players = self.__init_players_3p()
        game_manager._give_objective_cards(players)
        checked = set()
        for player in players:
            self.assertIsNot(checked.__contains__(player.objective_card),
                             "Duplicated objective cards assignment")
            checked.add(player.objective_card)

    def test_give_objective_cards_no_duplicates_4p(self):
        players = self.__init_players_4p()
        game_manager._give_objective_cards(players)
        checked = set()
        for player in players:
            self.assertIsNot(checked.__contains__(player.objective_card),
                             "Duplicated objective cards assignment")
            checked.add(player.objective_card)

    def test_give_objective_cards_no_duplicates_5p(self):
        players = self.__init_players_5p()
        game_manager._give_objective_cards(players)
        checked = set()
        for player in players:
            self.assertIsNot(checked.__contains__(player.objective_card),
                             "Duplicated objective cards assignment")
            checked.add(player.objective_card)

    def test_give_objective_cards_no_duplicates_6p(self):
        players = self.__init_players_6p()
        game_manager._give_objective_cards(players)
        checked = set()
        for player in players:
            self.assertIsNot(checked.__contains__(player.objective_card),
                             "Duplicated objective cards assignment")
            checked.add(player.objective_card)

    def test_give_territory_cards_3p(self):
        players = self.__init_players_3p()
        num_territories = len(utils.read_territories_cards())
        num_territories_each_player = num_territories / 3
        game_manager._give_territory_cards(players)
        for player in players:
            self.assertEqual(len(player.territories), num_territories_each_player,
                             "Incorrect territories assign")

    def test_give_territory_cards_4p(self):
        players = self.__init_players_4p()
        num_territories = len(utils.read_territories_cards())
        num_territories_each_player = num_territories // 4  # Only integer part is needed
        game_manager._give_territory_cards(players)
        self.assertEqual(len(players[0].territories), num_territories_each_player + 1,
                         "Incorrect territories assign")
        self.assertEqual(len(players[1].territories), num_territories_each_player + 1,
                         "Incorrect territories assign")
        self.assertEqual(len(players[2].territories), num_territories_each_player,
                         "Incorrect territories assign")
        self.assertEqual(len(players[3].territories), num_territories_each_player,
                         "Incorrect territories assign")

    def test_give_territory_cards_5p(self):
        players = self.__init_players_5p()
        num_territories = len(utils.read_territories_cards())
        num_territories_each_player = num_territories // 5
        game_manager._give_territory_cards(players)
        self.assertEqual(len(players[0].territories), num_territories_each_player + 1,
                         "Incorrect territories assign")
        self.assertEqual(len(players[1].territories), num_territories_each_player + 1,
                         "Incorrect territories assign")
        self.assertEqual(len(players[2].territories), num_territories_each_player,
                         "Incorrect territories assign")
        self.assertEqual(len(players[3].territories), num_territories_each_player,
                         "Incorrect territories assign")
        self.assertEqual(len(players[4].territories), num_territories_each_player,
                         "Incorrect territories assign")

    def test_give_territory_cards_6p(self):
        players = self.__init_players_6p()
        num_territories = len(utils.read_territories_cards())
        num_territories_each_player = num_territories / 6
        game_manager._give_territory_cards(players)
        for player in players:
            self.assertEqual(len(player.territories), num_territories_each_player,
                             "Incorrect territories assign")

    def test_give_territory_cards_right_player_3p(self):
        players = self.__init_players_3p()
        game_manager._give_territory_cards(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(player.player_id, territory.player_id,
                                 "Incorrect territories assign")

    def test_give_territory_cards_right_player_4p(self):
        players = self.__init_players_4p()
        game_manager._give_territory_cards(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(player.player_id, territory.player_id,
                                 "Incorrect territories assign")

    def test_give_territory_cards_right_player_5p(self):
        players = self.__init_players_5p()
        game_manager._give_territory_cards(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(player.player_id, territory.player_id,
                                 "Incorrect territories assign")

    def test_give_territory_cards_right_player_6p(self):
        players = self.__init_players_6p()
        game_manager._give_territory_cards(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(player.player_id, territory.player_id,
                                 "Incorrect territories assign")

    def test_give_territory_cards_no_duplicates_3p(self):
        players = self.__init_players_3p()
        game_manager._give_territory_cards(players)
        checked = set()
        for player in players:
            for territory in player.territories:
                self.assertIsNot(checked.__contains__(territory),
                                 "Duplicated territory cards assignment ")
                checked.add(territory)

    def test_give_territory_cards_no_duplicates_4p(self):
        players = self.__init_players_4p()
        game_manager._give_territory_cards(players)
        checked = set()
        for player in players:
            for territory in player.territories:
                self.assertIsNot(checked.__contains__(territory),
                                 "Duplicated territory cards assignment ")
                checked.add(territory)

    def test_give_territory_cards_no_duplicates_5p(self):
        players = self.__init_players_5p()
        game_manager._give_territory_cards(players)
        checked = set()
        for player in players:
            for territory in player.territories:
                self.assertIsNot(checked.__contains__(territory),
                                 "Duplicated territory cards assignment ")
                checked.add(territory)

    def test_give_territory_cards_no_duplicates_6p(self):
        players = self.__init_players_6p()
        game_manager._give_territory_cards(players)
        checked = set()
        for player in players:
            for territory in player.territories:
                self.assertIsNot(checked.__contains__(territory),
                                 "Duplicated territory cards assignment ")
                checked.add(territory)

    def test_assign_default_tanks_to_territories_3p(self):
        players = self.__init_players_3p()
        game_manager._assign_default_tanks_to_territories(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(territory.num_tanks, 1,
                                 "Error during default tanks assingment")
                self.assertEqual(player.tanks_placed, len(player.territories), "Error during default tanks assingment")
                self.assertEqual(player.tanks_available, player.tanks_num - player.tanks_placed,
                                 "Error during default tanks assingment")

    def test_assign_default_tanks_to_territories_4p(self):
        players = self.__init_players_4p()
        game_manager._assign_default_tanks_to_territories(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(territory.num_tanks, 1,
                                 "Error during default tanks assingment")
            self.assertEqual(player.tanks_placed, len(player.territories), "Error during default tanks assingment")
            self.assertEqual(player.tanks_available, player.tanks_num - player.tanks_placed,
                             "Error during default tanks assingment")

    def test_assign_default_tanks_to_territories_5p(self):
        players = self.__init_players_5p()
        game_manager._assign_default_tanks_to_territories(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(territory.num_tanks, 1,
                                 "Error during default tanks assingment")
            self.assertEqual(player.tanks_placed, len(player.territories), "Error during default tanks assingment")
            self.assertEqual(player.tanks_available, player.tanks_num - player.tanks_placed,
                             "Error during default tanks assingment")

    def test_assign_default_tanks_to_territories_6p(self):
        players = self.__init_players_6p()
        game_manager._assign_default_tanks_to_territories(players)
        for player in players:
            for territory in player.territories:
                self.assertEqual(territory.num_tanks, 1,
                                 "Error during default tanks assingment")
            self.assertEqual(player.tanks_placed, len(player.territories), "Error during default tanks assingment")
            self.assertEqual(player.tanks_available, player.tanks_num - player.tanks_placed,
                             "Error during default tanks assingment")


if __name__ == '__main__':
    unittest.main()
