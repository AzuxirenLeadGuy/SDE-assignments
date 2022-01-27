test: ./test/test.cpp  ./lib/Stack.cpp
	g++ -g ./test/test.cpp -o ./bin/test.out && ./bin/test.out

build: ./src/Program.cpp ./lib/Stack.cpp
	g++ ./src/Program.cpp -o ./bin/Program.out