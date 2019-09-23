using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfChatServiceLibrary
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Schnittstellennamen "IService1" sowohl im Code als auch in der Konfigurationsdatei ändern.
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatServiceInbound
    {
        [OperationContract]
        int JoinTheConversation(string userName);

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string userName, List<string> addressList, string userMessage);

        [OperationContract]
        int LeaveTheConversation(string userName);
        // TODO: Hier Dienstvorgänge hinzufügen
    }

    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyUserJoinedTheConversation(string userName);

        [OperationContract(IsOneWay = true)]
        void NotifyUserOfMessage(string userName, String userMessage);

        [OperationContract(IsOneWay = true)]
        void NotifyUserLeftTheConversation(string userName);
    }

    // Verwenden Sie einen Datenvertrag, wie im folgenden Beispiel dargestellt, um Dienstvorgängen zusammengesetzte Typen hinzuzufügen.
    // Sie können im Projekt XSD-Dateien hinzufügen. Sie können nach dem Erstellen des Projekts dort definierte Datentypen über den Namespace "WcfChatServiceLibrary.ContractType" direkt verwenden.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
