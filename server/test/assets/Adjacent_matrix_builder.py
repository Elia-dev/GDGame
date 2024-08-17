import numpy as np

n = 42

adj_matrix = np.zeros((n, n), dtype=int)

edges = [
    (0, 1), (0, 3), (0, 30),                                    # Alaska neighbors
    (1, 0), (1, 3), (1, 4), (1, 2),                             # Northwest neighbors
    (2, 1), (2, 4), (2, 5), (2, 13),                            # Greenland neighbors
    (3, 0), (3, 1), (3, 4), (3, 6),                             # Alberta neighbors
    (4, 1), (4, 3), (4, 6), (4, 7), (4, 5), (4, 2),             # Ontario neighbors
    (5, 4), (5, 2), (5, 7),                                     # Quebec neighbors
    (6, 3), (6, 4), (6, 7), (6, 8),                             # Western U.S. neighbors
    (7, 5), (7, 4), (7, 6), (7, 8),                             # Eastern U.S. neighbors
    (8, 6), (8, 7), (8, 9),                                     # Central America neighbors
    (9, 8), (9, 10), (9, 11),                                   # Venezuela neighbors
    (10, 9), (10, 11), (10, 12),                                # Peru neighbors
    (11, 9), (11, 10), (11, 12),                                # Brazil neighbors
    (12, 10), (12, 11),                                         # Argentina neighbors
    (13, 2), (13, 15), (13, 14),                                # Iceland neighbors
    (14, 13), (14, 15), (14, 19),                               # Scandinavia neighbors
    (15, 13), (15, 14), (15, 16), (15, 17),                     # Great Britain neighbors
    (16, 15), (16, 19), (16, 17), (16, 18),                     # Northern Europe neighbors
    (17, 15), (17, 16), (17, 18),                               # Western Europe neighbors
    (18, 17), (18, 16), (18, 19), (18, 34), (18, 21), (18, 20), # Southern Europe neighbors
    (19, 14), (19, 16), (19, 18), (19, 34), (19, 33), (19, 26), # Ukraine neighbors
    (20, 18), (20, 11), (20, 21), (20, 23), (20, 22),           # North Africa neighbors
    (21, 18), (21, 20), (21, 23), (21, 34),                     # Egypt neighbors
    (22, 20), (22, 23), (22, 24),                               # Congo neighbors
    (23, 34), (23, 21), (23, 20), (23, 22), (23, 24), (23, 25), # Eastern Africa neighbors
    (24, 22), (24, 23), (24, 25),                               # Southern Africa neighbors
    (25, 23), (25, 24),                                         # Madagascar neighbors
    (26, 19), (26, 33), (26, 36), (26, 27),                     # Ural neighbors
    (27, 26), (27, 36), (27, 32), (27, 29), (27, 28),           # Siberia neighbors
    (28, 27), (28, 29), (28, 30),                               # Yakutsk neighbors
    (29, 27), (29, 32), (29, 30), (29, 28),                     # Irkutsk neighbors
    (30, 28), (30, 29), (30, 32), (30, 31), (30, 0),            # Kamchatka neighbors
    (31, 30), (31, 32),                                         # Japan neighbors
    (32, 31), (32, 30), (32, 29), (32, 27), (32, 36),           # Mongolia neighbors
    (33, 26), (33, 19), (33, 34), (33, 35), (33, 36),           # Afghanistan neighbors
    (34, 19), (34, 18), (34, 21), (34, 35), (34, 33), (34, 23), # Middle East neighbors
    (35, 34), (35, 33), (35, 36), (35, 37),                     # India neighbors
    (36, 35), (36, 37), (36, 33), (36, 26), (36, 27), (36, 32), # China neighbors
    (37, 35), (37, 36), (37, 38),                               # Siam neighbors
    (38, 37), (38, 41), (38, 39),                               # Indonesia neighbors
    (39, 38), (39, 41), (39, 40),                               # New Guinea neighbors
    (40, 38), (40, 39), (40, 41),                               # Eastern Australia neighbors
    (41, 40), (41, 39)                                          # Western Australia neighbors
]

for (i, j) in edges:
    adj_matrix[i, j] = 1
    adj_matrix[j, i] = 1

np.save("adj_matrix.npy", adj_matrix)
