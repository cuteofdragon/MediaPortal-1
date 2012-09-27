using System.Collections.Generic;
using System.Linq;
using Mediaportal.TV.Server.TVDatabase.Entities;
using Mediaportal.TV.Server.TVDatabase.Entities.Enums;
using Mediaportal.TV.Server.TVDatabase.Entities.Factories;

namespace Mediaportal.TV.Server.TVService.ServiceAgents
{
  public static class MappingHelper
  {
    public static GroupMap AddChannelToGroup(ref Channel channel, ChannelGroup @group, MediaTypeEnum mediaType)
    {
      foreach (GroupMap groupMap in channel.GroupMaps.Where(groupMap => groupMap.idGroup == @group.IdGroup))
      {
        return groupMap;
      }
      DoAddChannelToGroup(channel, group, mediaType);
      channel = ServiceAgents.Instance.ChannelServiceAgent.SaveChannel(channel);
      channel.AcceptChanges();
      return channel.GroupMaps.FirstOrDefault(gMap => gMap.idGroup == @group.IdGroup);
    }

    private static void DoAddChannelToGroup(Channel channel, ChannelGroup @group, MediaTypeEnum mediaType)
    {
      var groupMap = new GroupMap
                       {
                         //Channel = channel, // causes : AcceptChanges cannot continue because the object's key values conflict with another object in the ObjectStateManager. Make sure that the key values are unique before calling AcceptChanges.
                         //ChannelGroup = group,
                         idChannel = channel.IdChannel,
                         idGroup = @group.IdGroup,
                         mediaType = (int) mediaType,
                         SortOrder = channel.SortOrder
                       };
      channel.GroupMaps.Add(groupMap);
    }

    public static GroupMap AddChannelToGroup(ref Channel channel, string groupName, MediaTypeEnum mediaType)
    {
      ChannelGroup channelGroup = ServiceAgents.Instance.ChannelGroupServiceAgent.GetChannelGroupByNameAndMediaType(groupName, mediaType);
      if (channelGroup != null)
      {
        return AddChannelToGroup(ref channel, channelGroup, mediaType);
      }
      return null;
    }

    public static void AddChannelsToGroup(IEnumerable<Channel> channels, ChannelGroup group)
    {
      foreach (Channel channel in channels)
      {
        DoAddChannelToGroup(channel, group, (MediaTypeEnum) channel.MediaType);
      }
      ServiceAgents.Instance.ChannelServiceAgent.SaveChannels(channels);
    }

    public static ChannelMap AddChannelToCard(Channel channel, Card card, bool epg)
    {      
      foreach (ChannelMap channelMap in channel.ChannelMaps.Where(chMap => chMap.idCard == card.IdCard))
      {
        //already associated ?
        return channelMap;
      }

      var map = new ChannelMap()
      {
        idChannel = channel.IdChannel,
        idCard =  card.IdCard,
        epgOnly = epg
      };
      
      channel.ChannelMaps.Add(map);
      channel = ServiceAgents.Instance.ChannelServiceAgent.SaveChannel(channel);
      channel.AcceptChanges();
      return channel.ChannelMaps.FirstOrDefault(chMap => chMap.idCard == card.IdCard);
    }
  
  }
}
