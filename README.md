Solvoku
=======

Sudoku Solver Unity Application

## Sudoku Solver Algorithms
- The following the algorithms considered for the Sudoku Solver:
	- Brute Force Algorithm
	- Backtracking
	- Exact Cover (extends Backtracking)
	- Stochastic search/optimization methods
		- Sounds fun more than efficient since it randomly places numbers into slots and finds "mistakes"
	- Constraint Programming
- NOTE: Must decide on which algorithms to actually program

## Solvoku design
- Display an empty 9x9 grid. Allow user to setup the sudoku problem.
- Have a large "solve" button below the solver.
- Allow the user to "select an algorithm" under the "nerdy stuff" tab.
- Show statistics of how efficient one solver is over another.
- Display the values in another color from the original sudoku problem.

## In the making
- Created User Interface
- Create solver:
	- Backtracking:
		- Researched algorithm and attempt to implement
		- Harder than expected
		- Cannot be used by itself
			- depth level goes through the roof!
			- Even for easy levels
		- Impossible to use by itself
	- Researched other methods
		- Exact Cover Method:
			- Algorithm X
				- Dancing Links
					- FIGURED OUT THE ALGORITHM! EXTREMELY DIFFICULT TO UNDERSTAND
					- Very efficient and works on MOST sudoku boards!
					- Still doesn't work for extremely difficult levels
					- http://sudopedia.enjoysudoku.com/Dancing_Links.html
					- https://www.ocf.berkeley.edu/~jchu/publicportal/sudoku/sudoku.paper.html
					- https://www.ocf.berkeley.edu/~jchu/publicportal/sudoku/0011047.pdf
					- http://garethrees.org/2007/06/10/zendoku-generation/
					- http://www.sudokuwiki.org/sudoku.htm 
- Added clear button
- Added delete button
- Updated visuals
- Added basic error handling:
	- Minimum amount of set cells.
	- No selected slot when inputting.
- Added Title
- Added deselection
- TODO:
	- Add more error handling?
	- Ads?!?!! hahaha
	- Write paper
	- prepare presentation
		- Show dancing links next to matrix representation