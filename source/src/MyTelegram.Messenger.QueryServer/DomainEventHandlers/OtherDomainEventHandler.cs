﻿using MyTelegram.Domain.Aggregates.PeerNotifySettings;
using MyTelegram.Domain.Events.PeerNotifySettings;
using MyTelegram.Messenger.Services.Caching;
using MyTelegram.Messenger.TLObjectConverters.Interfaces;
using MyTelegram.Services.Extensions;
using MyTelegram.Services.TLObjectConverters;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

public class OtherDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IResponseCacheAppService responseCacheAppService,
    IEventBus eventBus,
    ILogger<OtherDomainEventHandler> logger,
    ILayeredService<IUpdatesConverter> layeredUpdatesService,
    ILayeredService<IAuthorizationConverter> layeredAuthorizationService,
    ILayeredService<IUserConverter> layeredUserService,
    ICacheManager<GlobalPrivacySettingsCacheItem> cacheManager)
    : DomainEventHandlerBase(objectMessageSender,
            commandBus,
            idGenerator,
            ackCacheService,
            responseCacheAppService),
        ISubscribeSynchronousTo<SignInSaga, SignInSagaId, SignInSuccessEvent>,
        ISubscribeSynchronousTo<SignInSaga, SignInSagaId, SignUpRequiredEvent>,
        //ISubscribeSynchronousTo<AppCodeAggregate, AppCodeId, SignUpRequiredEvent>,
        //ISubscribeSynchronousTo<UpdatePinnedMessageSaga, UpdatePinnedMessageSagaId, UpdatePinnedMessageCompletedEvent>,
        //ISubscribeSynchronousTo<DeleteMessageSaga, DeleteMessageSagaId, DeleteMessagesCompletedEvent>,
        //ISubscribeSynchronousTo<DeleteMessageSaga2, DeleteMessageSaga2Id, DeleteMessagesCompletedEvent2>,
        ISubscribeSynchronousTo<ClearHistorySaga, ClearHistorySagaId, ClearSingleUserHistoryCompletedEvent>,
        //ISubscribeSynchronousTo<DeleteParticipantHistorySaga, DeleteParticipantHistorySagaId, DeleteParticipantHistoryCompletedEvent>,
        ISubscribeSynchronousTo<PeerNotifySettingsAggregate, PeerNotifySettingsId, PeerNotifySettingsUpdatedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserGlobalPrivacySettingsChangedEvent>,
        ISubscribeSynchronousTo<PinForwardedChannelMessageSaga, PinForwardedChannelMessageSagaId,
            PinChannelMessagePtsIncrementedEvent>,
        ISubscribeSynchronousTo<UpdatePinnedMessageSaga, UpdatePinnedMessageSagaId, UpdateSavedMessagesPinnedCompletedEvent>//,
                                                                                                                            //ISubscribeSynchronousTo<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllMessagesCompletedSagaEvent>,
                                                                                                                            //ISubscribeSynchronousTo<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllParticipantMessagesCompletedSagaEvent>

//,
//ISubscribeSynchronousTo<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteSelfMessagesCompletedEvent4>,
//ISubscribeSynchronousTo<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteOtherParticipantMessagesCompletedEvent4>,
//ISubscribeSynchronousTo<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteSelfHistoryCompletedEvent4>,
//ISubscribeSynchronousTo<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteOtherParticipantHistoryCompletedEvent4>,
//ISubscribeSynchronousTo<DeleteChannelMessagesSaga,DeleteChannelMessagesSagaId, DeleteChannelMessagesCompletedEvent>

{
    private readonly IObjectMessageSender _objectMessageSender = objectMessageSender;

    public async Task HandleAsync(
        IDomainEvent<ClearHistorySaga, ClearHistorySagaId, ClearSingleUserHistoryCompletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.IsSelf)
        {
            var r = new TAffectedHistory
            {
                Pts = domainEvent.AggregateEvent.DeletedBoxItem.Pts,
                PtsCount = domainEvent.AggregateEvent.DeletedBoxItem.PtsCount,
                Offset = domainEvent.AggregateEvent.NextMaxId
            };
            await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
                r
            );
        }

        var date = DateTime.UtcNow.ToTimestamp();
        var updates = layeredUpdatesService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
            .ToDeleteMessagesUpdates(domainEvent.AggregateEvent.ToPeerType,
                domainEvent.AggregateEvent.DeletedBoxItem,
                date);
        var layeredData = layeredUpdatesService.GetLayeredData(c =>
            c.ToDeleteMessagesUpdates(domainEvent.AggregateEvent.ToPeerType,
                domainEvent.AggregateEvent.DeletedBoxItem,
                date));
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, domainEvent.AggregateEvent.DeletedBoxItem.OwnerPeerId),
            updates,
            pts: domainEvent.AggregateEvent.DeletedBoxItem.Pts,
            layeredData: layeredData);
    }

    public async Task HandleAsync(IDomainEvent<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteSelfMessagesCompletedEvent4> domainEvent, CancellationToken cancellationToken)
    {
        var r = new TAffectedMessages
        {
            Pts = domainEvent.AggregateEvent.Pts,
            PtsCount = domainEvent.AggregateEvent.PtsCount
        };
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
            r,
            domainEvent.AggregateEvent.RequestInfo.UserId, domainEvent.AggregateEvent.Pts);

        var selfOtherDeviceUpdates = layeredUpdatesService.Converter.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.RequestInfo.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp());
        await PushUpdatesToPeerAsync(domainEvent.AggregateEvent.RequestInfo.UserId.ToUserPeer(), selfOtherDeviceUpdates,
            domainEvent.AggregateEvent.RequestInfo.AuthKeyId);
    }

    public Task HandleAsync(IDomainEvent<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteOtherParticipantMessagesCompletedEvent4> domainEvent, CancellationToken cancellationToken)
    {
        var updates = layeredUpdatesService.Converter.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp());
        var layeredUpdates = layeredUpdatesService.GetLayeredData(c => c.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp()));

        return PushUpdatesToPeerAsync(domainEvent.AggregateEvent.UserId.ToUserPeer(), updates,
            pts: domainEvent.AggregateEvent.Pts, layeredData: layeredUpdates);

    }

    public async Task HandleAsync(IDomainEvent<SignInSaga, SignInSagaId, SignInSuccessEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var tempAuthKeyId = domainEvent.AggregateEvent.TempAuthKeyId;
        await eventBus.PublishAsync(new UserSignInSuccessEvent(
                domainEvent.AggregateEvent.RequestInfo.ReqMsgId,
                tempAuthKeyId,
                domainEvent.AggregateEvent.PermAuthKeyId,
                domainEvent.AggregateEvent.UserId,
                domainEvent.AggregateEvent.HasPassword ? PasswordState.WaitingForVerify : PasswordState.None))
     ;
        logger.LogDebug(
            "########################### User sign in success:{UserId} with tempAuthKeyId:{TempAuthKeyId} permAuthKeyId:{PermAuthKeyId} layer:{Layer}",
            domainEvent.AggregateEvent.UserId,
            domainEvent.AggregateEvent.TempAuthKeyId,
            domainEvent.AggregateEvent.PermAuthKeyId,
            domainEvent.AggregateEvent.RequestInfo.Layer
            );


        if (domainEvent.AggregateEvent.HasPassword)
        {
            //var rpcError = new TRpcError
            //{
            //    ErrorCode = MyTelegramServerDomainConsts.BadRequestErrorCode,
            //    ErrorMessage = RpcErrors.RpcErrors401.SessionPasswordNeeded.Message,
            //};

            await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, RpcErrors.RpcErrors401.SessionPasswordNeeded.ToRpcError());
            return;
        }

        var user = layeredUserService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
            .ToUser(domainEvent.AggregateEvent);

        var r = layeredAuthorizationService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
            .CreateAuthorization(user);

        await _objectMessageSender.SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo.ReqMsgId,
            r,
            domainEvent.AggregateEvent.RequestInfo.AuthKeyId,
            domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId,
            user.Id);
    }

    public Task HandleAsync(IDomainEvent<SignInSaga, SignInSagaId, SignUpRequiredEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        return SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
            layeredAuthorizationService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
                .CreateSignUpAuthorization());
    }

    public async Task HandleAsync(
        IDomainEvent<UpdatePinnedMessageSaga, UpdatePinnedMessageSagaId, UpdatePinnedMessageCompletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var r = layeredUpdatesService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
            .ToSelfUpdatePinnedMessageUpdates(domainEvent.AggregateEvent);
        if (domainEvent.AggregateEvent.PmOneSide || domainEvent.AggregateEvent.ShouldReplyRpcResult)
        {
            await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
                r,
                domainEvent.AggregateEvent.SenderPeerId,
                domainEvent.AggregateEvent.Pts,
                //PtsType.OtherUpdates,
                domainEvent.AggregateEvent.ToPeer.PeerType
            );
            var layeredData =
                layeredUpdatesService.GetLayeredData(c =>
                    c.ToSelfUpdatePinnedMessageUpdates(domainEvent.AggregateEvent));
            await PushUpdatesToPeerAsync(
                new Peer(PeerType.User, domainEvent.AggregateEvent.OwnerPeerId),
                r,
                pts: domainEvent.AggregateEvent.Pts,
                layeredData: layeredData);
        }

        //else
        {

            await PushUpdatesToPeerAsync(
                domainEvent.AggregateEvent.ToPeer.PeerType == PeerType.Channel
                    ? new Peer(PeerType.Channel, domainEvent.AggregateEvent.OwnerPeerId)
                    : new Peer(PeerType.User, domainEvent.AggregateEvent.OwnerPeerId),
                layeredUpdatesService.Converter.ToUpdatePinnedMessageUpdates(domainEvent.AggregateEvent),
                excludeUserId: domainEvent.AggregateEvent.SenderPeerId,
                pts: domainEvent.AggregateEvent.Pts,
                layeredData: layeredUpdatesService.GetLayeredData(c =>
                    c.ToUpdatePinnedMessageUpdates(domainEvent.AggregateEvent))
            );
        }
    }

    public Task HandleAsync(IDomainEvent<PeerNotifySettingsAggregate, PeerNotifySettingsId, PeerNotifySettingsUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var r = new TBoolTrue();

        return SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, r);
    }

    public Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserGlobalPrivacySettingsChangedEvent> domainEvent, CancellationToken cancellationToken)
    {
        return cacheManager.SetAsync(
            GlobalPrivacySettingsCacheItem.GetCacheKey(domainEvent.AggregateEvent.RequestInfo.UserId),
            new GlobalPrivacySettingsCacheItem(domainEvent.AggregateEvent.GlobalPrivacySettings.ArchiveAndMuteNewNoncontactPeers,
                domainEvent.AggregateEvent.GlobalPrivacySettings.KeepArchivedUnmuted,
                domainEvent.AggregateEvent.GlobalPrivacySettings.KeepArchivedFolders,
                domainEvent.AggregateEvent.GlobalPrivacySettings.HideReadMarks,
                domainEvent.AggregateEvent.GlobalPrivacySettings.NewNoncontactPeersRequirePremium
                )
        );
    }

    public async Task HandleAsync(IDomainEvent<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteSelfHistoryCompletedEvent4> domainEvent, CancellationToken cancellationToken)
    {
        var r = new TAffectedHistory
        {
            Pts = domainEvent.AggregateEvent.Pts,
            PtsCount = domainEvent.AggregateEvent.PtsCount,
            Offset = domainEvent.AggregateEvent.Offset
        };
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, r);
        var selfOtherDeviceUpdates = layeredUpdatesService.Converter.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.RequestInfo.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp());

        await PushUpdatesToPeerAsync(domainEvent.AggregateEvent.RequestInfo.UserId.ToUserPeer(), selfOtherDeviceUpdates,
            domainEvent.AggregateEvent.RequestInfo.AuthKeyId);
    }

    public Task HandleAsync(IDomainEvent<DeleteMessagesSaga4, DeleteMessagesSaga4Id, DeleteOtherParticipantHistoryCompletedEvent4> domainEvent, CancellationToken cancellationToken)
    {
        var updates = layeredUpdatesService.Converter.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp());
        var layeredUpdates = layeredUpdatesService.GetLayeredData(c => c.ToDeleteMessagesUpdates(PeerType.User,
            new DeletedBoxItem(domainEvent.AggregateEvent.UserId, domainEvent.AggregateEvent.Pts,
                domainEvent.AggregateEvent.PtsCount, domainEvent.AggregateEvent.MessageIds),
            DateTime.UtcNow.ToTimestamp()));

        return PushUpdatesToPeerAsync(domainEvent.AggregateEvent.UserId.ToUserPeer(), updates,
            pts: domainEvent.AggregateEvent.Pts, layeredData: layeredUpdates);
    }

    public Task HandleAsync(IDomainEvent<PinForwardedChannelMessageSaga, PinForwardedChannelMessageSagaId, PinChannelMessagePtsIncrementedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var updateShort = new TUpdateShort
        {
            Update = new TUpdatePinnedChannelMessages
            {
                ChannelId = domainEvent.AggregateEvent.ChannelId,
                Messages = [domainEvent.AggregateEvent.MessageId],
                Pinned = true,
                Pts = domainEvent.AggregateEvent.Pts,
                PtsCount = 1
            },
            Date = DateTime.UtcNow.ToTimestamp(),
        };

        return PushMessageToPeerAsync(domainEvent.AggregateEvent.ChannelId.ToChannelPeer(), updateShort);
    }

    public async Task HandleAsync(IDomainEvent<UpdatePinnedMessageSaga, UpdatePinnedMessageSagaId, UpdateSavedMessagesPinnedCompletedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var updatePinnedMessages = new TUpdatePinnedMessages
        {
            Pinned = domainEvent.AggregateEvent.Pinned,
            Peer = new TPeerUser
            {
                UserId = domainEvent.AggregateEvent.RequestInfo.UserId
            },
            Messages = new TVector<int>(domainEvent.AggregateEvent.MessageIds),
            Pts = domainEvent.AggregateEvent.Pts,
            PtsCount = 1
        };
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(updatePinnedMessages),
            Users = new(),
            Chats = new(),
            Date = DateTime.UtcNow.ToTimestamp(),
        };
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, updates,
            pts: domainEvent.AggregateEvent.Pts);

        await PushUpdatesToPeerAsync(domainEvent.AggregateEvent.RequestInfo.UserId.ToUserPeer(), updates,
            domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId, pts: domainEvent.AggregateEvent.Pts);

    }
}
