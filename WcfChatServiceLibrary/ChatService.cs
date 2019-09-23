using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfChatServiceLibrary
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Klassennamen "Service1" sowohl im Code als auch in der Konfigurationsdatei ändern.
    public class ChatServiceInbound : IChatServiceInbound
    {
        private static List<IChatServiceCallback> _callbackList = new List<IChatServiceCallback>();
        //  number of current users - 0 to begin with
        private static int _registeredUsers = 0;

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public int JoinTheConversation(string userName)
        {
            // Subscribe the user to the conversation
            IChatServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            if (!_callbackList.Contains(registeredUser))
            {
                _callbackList.Add(registeredUser);
            }

            _callbackList.ForEach(
                delegate (IChatServiceCallback callback)
                {
                    callback.NotifyUserJoinedTheConversation(userName);                    
                });
            _registeredUsers++;
            return _registeredUsers;
        }

        public int LeaveTheConversation(string userName)
        {
            // Unsubscribe the user from the conversation.      
            IChatServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
                _registeredUsers--;
            }

            // Notify everyone that user has arrived.
            // Use an anonymous delegate and generics to do our dirty work.
            _callbackList.ForEach(
                delegate (IChatServiceCallback callback)
                { callback.NotifyUserLeftTheConversation(userName); });

            return _registeredUsers;
        }

        public void ReceiveMessage(string userName, List<string> addressList, string userMessage)
        {
            // Notify the users of a message.
            // Use an anonymous delegate and generics to do our dirty work.
            _callbackList.ForEach(
                delegate (IChatServiceCallback callback)
                { callback.NotifyUserOfMessage(userName, userMessage); });
        }
    }
}
