﻿using System.Net.WebSockets;
using System.Text;

var ws = new ClientWebSocket();

string name;
while (true)
{
    Console.OutputEncoding = Encoding.Unicode; // viết tiếng Việt
    Console.InputEncoding = Encoding.Unicode;
    Console.WriteLine("Nhập tên: ");
    name = Console.ReadLine();
    break;
}
Console.WriteLine("Đang kết nối tới server!");
await ws.ConnectAsync(new Uri($"ws://localhost:6969/ws?name={name}"), CancellationToken.None);
Console.WriteLine("Đã kết nối tới server!");

var sendTask = Task.Run(async () =>
{
    while (true)
    {
        var message = Console.ReadLine();
        if (message == "exit")
        {
            break;
        }
        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
});



var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024 * 4];

    while (true)
    {
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine("Received: " + message);
    }
});

await Task.WhenAny(sendTask, receiveTask);
if (ws.State != WebSocketState.Closed)
{
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(sendTask, receiveTask);
