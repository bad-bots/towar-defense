#!/bin/bash
docker run \
	-p "4574:8080" \
	-w "/usr/src/app" \
	--env-file "./.env.production" \
	--name "towar-defense" \
	node "/bin/bash"