
SET CMD=%1

IF "%CMD%"=="start-redis" GOTO START
IF "%CMD%"=="stop-redis" GOTO STOP
GOTO HELP

:START
echo "Initializing redis"
docker run --name flags-net -p 6379:6379 -d redis
GOTO END

:STOP
docker stop flags-net
GOTO END

:HELP
echo "flags.cmd (start-redis|stop-redis)"


:END
exit