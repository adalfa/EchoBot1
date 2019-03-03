﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace EchoBot1
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class EchoBot1Bot : IBot
    {
        private readonly EchoBot1Accessors _accessors;
        private readonly ILogger _logger;
        private PassaState _passa;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="conversationState">The managed conversation state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public EchoBot1Bot(ConversationState conversationState, ILoggerFactory loggerFactory)
        {
            if (conversationState == null)
            {
                throw new System.ArgumentNullException(nameof(conversationState));
            }

            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _accessors = new EchoBot1Accessors(conversationState)
            {
                CounterState = conversationState.CreateProperty<CounterState>(EchoBot1Accessors.CounterStateName),
                PassaState = conversationState.CreateProperty<PassaState>(EchoBot1Accessors.PassaStateName),
            };

            _logger = loggerFactory.CreateLogger<EchoBot1Bot>();
            _logger.LogTrace("Turn start.");
            
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                work(turnContext);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
            }
        }
        private async Task Ronfa(ITurnContext it,int c)
        {


            Random rnd = new Random();
            int dur = rnd.Next(1, c);
                        Thread.Sleep(dur * 1000);
            var passa = await _accessors.PassaState.GetAsync(it, () => new PassaState());
            passa.Passa = 1;
            await _accessors.PassaState.SetAsync(it, passa);

        }


        private async Task work(ITurnContext turnContext)
        {
            // Get the conversation state from the turn context.
            var state = await _accessors.CounterState.GetAsync(turnContext, () => new CounterState());
            var passa = await _accessors.PassaState.GetAsync(turnContext, () => new PassaState());
            passa.Passa = 0;
            await _accessors.PassaState.SetAsync(turnContext, passa);
            // Bump the turn count for this conversation.
            state.TurnCount++;
            Activity isTypingActivity = new Activity();
            isTypingActivity.Type = ActivityTypes.Handoff;
            await turnContext.SendActivityAsync(isTypingActivity);

            clessidra(turnContext);
            Ronfa(turnContext, 30);
           
            // Set the property using the accessor.
            await _accessors.CounterState.SetAsync(turnContext, state);

            // Save the new turn count into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext);

            // Echo back to the user whatever they typed.
            var responseMessage = $"Turn {state.TurnCount}: hai inviato '{turnContext.Activity.Text}'\n";
            await turnContext.SendActivityAsync(responseMessage);

        }

        private async Task clessidra( ITurnContext it)
        {
            Activity isTypingActivity = new Activity();
            isTypingActivity.Type = ActivityTypes.Typing;
            bool flag = true;
           while (flag)
            { 
                await it.SendActivityAsync((Activity)isTypingActivity);
                Thread.Sleep(5000);
                var passa = await _accessors.PassaState.GetAsync(it, () => new PassaState());
                if (passa.Passa == 1)
                    flag = false;

            }
        }

    }

}
