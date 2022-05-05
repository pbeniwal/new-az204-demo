﻿using System;


using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace producer
{
    class Program
    {
        
        // connection string to your Service Bus namespace
        static string connectionString = "Endpoint=sb://mydemosvcbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ihVlTNJetxedy6pA128fyrpdUpTosJ+Mg9PkYl+VOW4=";
        // name of your Service Bus topic
        static string queueName = "demo";

        // the client that owns the connection and can be used to create senders and receivers
        static ServiceBusClient client;
        // the sender used to publish messages to the queue
        static ServiceBusSender sender;
        // number of messages to be sent to the queue
        private const int numOfMessages = 10;        
        
        static async Task Main()
        {
            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(queueName);

            // create a batch
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= 10; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    // if an exception occurs
                    throw new Exception($"Exception {i} has occurred.");
                }
            }
            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }
    }
}
