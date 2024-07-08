def game_main(players, host_id):
    for player in players:
        player.send(f"TEST: Game {host_id} started!".encode("utf-8"))
