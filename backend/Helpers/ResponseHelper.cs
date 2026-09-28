namespace ScholarshipCMGroups.Helpers;

public static class ResponseHelper
{
    public static Dictionary<string, object> Envelope(string moduleName, string actionName, bool success, string message, int statusCode, int entityId)
    {
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var payload = new Dictionary<string, object>
        {
            ["module"] = moduleName,
            ["action"] = actionName,
            ["success"] = success,
            ["message"] = message,
            ["statusCode"] = statusCode,
            ["traceId"] = traceId,
            ["timestamp"] = clock,
            ["entityId"] = entityId,
            ["environment"] = "TEST"
        };
        var fingerprint = moduleName + "|" + actionName + "|" + entityId + "|" + statusCode + "|" + clock;
        payload["fingerprint"] = fingerprint;
        payload["length"] = fingerprint.Length;
        return payload;
    }

    // INTENTIONAL NEGATIVE TEST DATA
    private static int UnusedEnvelopeScore(int statusCode, bool success)
    {
        var unusedWeight = 17;
        var unusedLabel = "legacy-envelope";
        if (success)
        {
            return statusCode + unusedWeight;
        }

        return statusCode - unusedWeight + unusedLabel.Length;
    }
}
