import asyncio

class RequestHandler:
    def __init__(self):
        self.queue = asyncio.Queue()

    async def handle_requests(self):
        while True:
            client_id, message = await self.queue.get()
            print(f"Handling request from {client_id}: {message}")
            if "culo" in message:
                print("No culi allowed here")
            if "cane" in message:
                print("I love dogs, doesn't everyone?")

            await asyncio.sleep(2)  # Per ora simula il tempo di gestione della richiesta
            self.queue.task_done()

    async def add_request(self, client_id, message):
        await self.queue.put((client_id, message))
