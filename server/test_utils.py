import unittest
import utils

class MyTestCase(unittest.TestCase):
    def test_read_objects_cards(self):
        cards = utils.read_objects_cards()
        self.assertEqual(cards[0].id, "obj1")
        self.assertEqual(cards[1].id, "obj2")
        self.assertEqual(cards[2].id, "obj3")
        self.assertEqual(cards[3].id, "obj4")
        self.assertEqual(cards[4].id, "obj5")
        self.assertEqual(cards[5].id, "obj6")
        self.assertEqual(cards[6].id, "obj7")
        self.assertEqual(cards[7].id, "obj8")
        self.assertEqual(cards[8].id, "obj9")

    def test_read_territories_cards(self):
        cards = utils.read_territories_cards()
        self.assertEqual(cards[0].id, "NA_ter1")
        self.assertEqual(cards[1].id, "NA_ter2")
        self.assertEqual(cards[2].id, "NA_ter3")
        self.assertEqual(cards[3].id, "NA_ter4")
        self.assertEqual(cards[4].id, "NA_ter5")
        self.assertEqual(cards[5].id, "NA_ter6")
        self.assertEqual(cards[6].id, "NA_ter7")
        self.assertEqual(cards[7].id, "NA_ter8")
        self.assertEqual(cards[8].id, "NA_ter9")

        self.assertEqual(cards[9].id, "SA_ter1")
        self.assertEqual(cards[10].id, "SA_ter2")
        self.assertEqual(cards[11].id, "SA_ter3")
        self.assertEqual(cards[12].id, "SA_ter4")

        self.assertEqual(cards[13].id, "EU_ter1")
        self.assertEqual(cards[14].id, "EU_ter2")
        self.assertEqual(cards[15].id, "EU_ter3")
        self.assertEqual(cards[16].id, "EU_ter4")
        self.assertEqual(cards[17].id, "EU_ter5")
        self.assertEqual(cards[18].id, "EU_ter6")
        self.assertEqual(cards[19].id, "EU_ter7")

        self.assertEqual(cards[20].id, "AF_ter1")
        self.assertEqual(cards[21].id, "AF_ter2")
        self.assertEqual(cards[22].id, "AF_ter3")
        self.assertEqual(cards[23].id, "AF_ter4")
        self.assertEqual(cards[24].id, "AF_ter5")
        self.assertEqual(cards[25].id, "AF_ter6")

        self.assertEqual(cards[26].id, "AS_ter1")
        self.assertEqual(cards[27].id, "AS_ter2")
        self.assertEqual(cards[28].id, "AS_ter3")
        self.assertEqual(cards[29].id, "AS_ter4")
        self.assertEqual(cards[30].id, "AS_ter5")
        self.assertEqual(cards[31].id, "AS_ter6")
        self.assertEqual(cards[32].id, "AS_ter7")
        self.assertEqual(cards[33].id, "AS_ter8")
        self.assertEqual(cards[34].id, "AS_ter9")
        self.assertEqual(cards[35].id, "AS_ter10")
        self.assertEqual(cards[36].id, "AS_ter11")
        self.assertEqual(cards[37].id, "AS_ter12")

        self.assertEqual(cards[38].id, "OC_ter1")
        self.assertEqual(cards[39].id, "OC_ter2")
        self.assertEqual(cards[40].id, "OC_ter3")
        self.assertEqual(cards[41].id, "OC_ter4")

    def test_adj_matrix(self):
        adj_matrix = utils.get_adj_matrix()
        self.assertEqual(adj_matrix[0][1], 1)

    def test_retrieve_neighbors(self):
        neighbors = utils.get_neighbors_of(0)
        self.assertEqual(neighbors, [1, 3, 30])


if __name__ == '__main__':
    unittest.main()
