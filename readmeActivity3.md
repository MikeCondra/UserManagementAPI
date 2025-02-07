# README FOR ACTIVITY 3

Three handlers were added: ExceptionHandlingMiddleware, TokenSimulationMiddleware, RequestResponseLoggingMiddleware.

Requests exist in UserManagementAPI.http to exercise all of these.

ExceptionHandlingMiddleware catches all uncaught exceptions. Test with the following endpoint.  Be sure in VS to uncheck breakpoint all exceptions.

    GET{{UserManagementAPI_HostAddress}}/generate-exception

TokenSimulationMiddleware simulates an Authorization token enabled for GET, PUT, POST, DELETE. It is by default on. To test not having a token, append "?simulateToken=false" to the request.

    GET{{UserManagementAPI_HostAddress}}/?simulateToken=false fails

    GET{{UserManagementAPI_HostAddress}}/ returns "I am root!"

RequestResposneLoggingMiddleware puts request and response info into a data structure. It can be dumped with this request:

    GET{{UserManagementAPI_HostAddress}}/logs

