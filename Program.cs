using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using MakingBot.Commands;
using MakingBot.config;
using DSharpPlus.EventArgs;
using System;
using System.Runtime.Serialization;
using PuppeteerSharp;

namespace MakingBot
{
    internal class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {
            JSONReader jsonReader = new();
            await jsonReader.ReadJSON();

            DiscordConfiguration discordConfig = new()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2),
            });

            Client.Ready += OnClientReady;
            //Client.MessageCreated += MessageCreatedHandler;
            Client.VoiceStateUpdated += VoiceChannelEntered;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {jsonReader.Prefix},
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Utility>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static async Task VoiceChannelEntered(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Before == null && e.Channel.Name == "Bot Channel")
                await e.Channel.SendMessageAsync($"{e.User.Username} entered the vc.");

        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if(!e.Author.IsBot)
            await e.Channel.SendMessageAsync($"{e.Author.Username} sent a message.");
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
