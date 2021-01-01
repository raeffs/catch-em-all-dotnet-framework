using Cirrus.Logging;

namespace Cirrus.Module.CatchEmAll
{
    internal static class CLMTIDs
    {
        internal static readonly LogMessageTypeId CouldNotUpdateQuery = new LogMessageTypeId("CEA-00001");
        internal static readonly LogMessageTypeId CouldNotUpdateResult = new LogMessageTypeId("CEA-00002");
        internal static readonly LogMessageTypeId CouldNotSendEmail = new LogMessageTypeId("CEA-00003");
        internal static readonly LogMessageTypeId UnexpectedErrorWhileUpdatingResult = new LogMessageTypeId("CEA-00004");
        internal static readonly LogMessageTypeId UnexpectedErrorWhileUpdatingQuery = new LogMessageTypeId("CEA-00005");
        internal static readonly LogMessageTypeId CouldNotUpdateResultOfQuery = new LogMessageTypeId("CEA-00006");
        internal static readonly LogMessageTypeId UnexpectedErrorWhileUpdatingResultOfQuery = new LogMessageTypeId("CEA-00007");
        internal static readonly LogMessageTypeId CouldNotLoadResults = new LogMessageTypeId("CEA-00008");
        internal static readonly LogMessageTypeId CouldNotSaveResults = new LogMessageTypeId("CEA-00009");
        internal static readonly LogMessageTypeId CouldNotTriggerIftttWebhook = new LogMessageTypeId("CEA-00010");
        internal static readonly LogMessageTypeId CouldNotDeleteOldQueryExecutions = new LogMessageTypeId("CEA-00011");
        internal static readonly LogMessageTypeId CouldNotDeleteOldResultExecutions = new LogMessageTypeId("CEA-00012");
    }
}
