test: ./test/test.cpp  ./lib/Stack.cpp
	g++ -g ./test/test.cpp -o ./bin/test.out && ./bin/test.out && rm ./bin/test.out && g++ -c ./src/Program.cpp && rm Program.o && echo "Integration complete!\n\n"

build: ./src/Program.cpp ./lib/Stack.cpp
	g++ ./src/Program.cpp -o ./bin/Program.out && echo "Deployment complete!\n\n"