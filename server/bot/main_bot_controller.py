from bot.Client_manager import ClientManager

if __name__ == '__main__':
    client_manager = ClientManager()
    client_manager.start_client()
    client_manager.player.name = 'Python_BOT'
    client_manager.player.lobby_id = ''
