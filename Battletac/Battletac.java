/* 	Battletac - Commando X's vs O's
	Brady Schnell
	Version 1.0
	Dec 05 2014 
	Last Updated: Aug 20 2015
	
	**** Work in progress list: *****
	Scan board - diag's are not working!!!! 
	Computer Diagonal strat is in progress
	Item selection for computer
	**********************************
	
	
	Break item use up into 2 categories 
	defensive - traps, decoys,
	offensive - missile, nuke, swap
	defensive are used when no danger is posed r <= 2
	n = 1 or 2 -> Swap, trap, Decoy
	offensives are used to stop the player from winning
	n = 3 - go for Missile, swap or 
	n > 4 = try everything to break up the players parts
	then as last resort, try to block
*/
import java.util.*;
import java.util.Random;
public class Battletac {
    
    /* Computers first move - 40% chance to place randomly on board, 60% chance to place it at 3,3.
        if there is something in 3,3 then yolo puts it anywhere else. */
	public static void firstComp(char[][] board, int[] comp) {
			Random n = new Random();
			int random = n.nextInt(10);
			if (random <= 4) {
				int x,y;
				do {
					x = n.nextInt(board.length-1);
					y = n.nextInt(board.length-1);
				} while(!validCompMove(board,x,y));
				board[x][y] = 'O';
				comp[1] = x; comp[2] = y; // Sets move trackers 
				comp[3] = x; comp[4] = y; // Sets original move trackers
				return;
			} else {
				if (validCompMove(board,2,2) == true) {
					board[2][2] = 'O';
					comp[1] = 2; comp[2] = 2;
					comp[3] = 2; comp[4] = 2; // Sets original move trackers
					return;
				} else {
					int x,y;
					do {
						x = n.nextInt(board.length-1);
						y = n.nextInt(board.length-1);
					} while(!validCompMove(board,x,y));
                    comp[1] = x; comp[2] = y;
					comp[3] = x; comp[4] = y; // Sets original move trackers
					board[x][y] = 'O';
					return;
				}
			}
	}
	public static void compItem(char[][] board, int[] points) {
		
	}
	public static void comp(int[] points, char[][] board, int[] comp) {
		int[] danger = new int[3]; // [0] = number in a row, [1] = coordinate of respective row or col. [2] = 1 - H, 2 - V, 3 - D
		scanBoard(board, danger, 'X'); // Computer scans board for x's in a row to decide what action should be taken.
		/* less than 2 is no real threat, computer tries to win the game. */
        //compItem(board,points);
		// check rescan board for own markers to find where to put next piece
		if (danger[0] < 2) { // No danger here, computer tries to win the game
            Random n = new Random(); // Random variable holders
			for(;;){
                if (comp[0] == -1 ) { // Strat doesn't exist, Computer must generate a new strat
                    comp[0] = 2; // 0 = Horizontally, 1 = Vertically, 2 = diagonal ------------ CHANGED TO ONLY DO DIAGONAL CURRENT
                }
                if (comp[0] == 0) {// attempt to place next marker on either side of existing place Horizontally.
                    int move = n.nextInt(2); 
					System.out.println("X move : " + comp[1] + "Y move : " + comp[2]);
                    if (move == 0 && validCompMove(board,comp[1],comp[2]+1)) { // attempt to place to the right
                        board[comp[1]][comp[2]+1] = 'O';
						comp[2]++;
						break;
                    } else if (validCompMove(board,comp[1],comp[2]-1)){ // attempt to place to the left
						board[comp[1]][comp[2]-1] = 'O';
						comp[2]--;
						break;
					} else if (validCompMove(board,comp[1],comp[2]+1)) { // attempt to place to the right
						System.out.println("placing to the right 2");
                        board[comp[1]][comp[2]+1] = 'O';
						comp[2]++;
						break;
                    } else if (validCompMove(board,comp[3],comp[4]-1)){ // Place left of original move
						board[comp[3]][comp[4]-1] = 'O'; 
						comp[1] = comp[3];
						comp[2] = comp[4]-1;
						break;
					} else if (validCompMove(board,comp[3],comp[4]+1)){ // Place right of original move
						board[comp[3]][comp[4]+1] = 'O';
						comp[1] = comp[3];
						comp[2] = comp[4]+1;
						break;
					} else { // was not possible, need to start new strat
					/* 	these statements will need to be modified later to more intuitively find a place to put markers
						so that maximizes number in a row. */
                        firstComp(board, comp);
                        comp[0] = -1; // resets strat
						break;
                    }
                } else if (comp[0] == 1) { // attempt to place marker vertically to win
                    int move = n.nextInt(2);
                    if (move == 0 && validCompMove(board,comp[1]-1,comp[2])){ // attempt to place above 
                        board[comp[1]-1][comp[2]] = 'O';
						comp[1]--;
						break;
                    } else if (validCompMove(board,comp[1]+1,comp[2])){ // attempt to place below
                        board[comp[1]+1][comp[2]] = 'O';
						comp[1]++;
						break;
					} else if (validCompMove(board, comp[1]-1,comp[2])){
						board[comp[1]-1][comp[2]] = 'O';
						comp[1]--;
						break;
					} else if (validCompMove(board,comp[3]-1, comp[4])){
						board[comp[3]-1][comp[4]] = 'O';
						comp[1] = comp[3]-1;
						comp[2] = comp[4];
						break;
					} else if (validCompMove(board,comp[3]+1, comp[4])){
						board[comp[3]+1][comp[4]] = 'O';
						comp[1] = comp[3]+1;
						comp[2] = comp[4];
						break;
                    } else { // New strat needed.
					/* 	these statements will need to be modified later to more intuitively find a place to put markers
						so that maximizes number in a row. */
                        firstComp(board, comp);
                        comp[0] = -1;
						break;						
                    }
                } else if (comp[0] == 2) { 
					/* ---------------- MAIN WORK IN PROGRESS AREA AS OF AUG 21 2015 ---------------------- */
                    System.out.println("Strat is diagonal, Still in progress!");
					if (board[2][2] == 'O') { // computer will only try to win diagonally if it holds middle part
						if (comp[5] == -1) { // No diagonal strat assigned, create one
							comp[5] = n.nextInt(2);
						}
						if (comp[5] == 0) { 
							int move = n.nextInt(2);
							if (move == 0 && validCompMove(board,comp[1]-1,comp[2]-1)){ 
								board[comp[1]-1][comp[2]-1] = 'O';
								comp[1]--;
								comp[2]--;
								break;
							} else if (validCompMove(board,comp[1]+1,comp[2]+1)){ 
								board[comp[1]+1][comp[2]+1] = 'O';
								comp[1]++;
								comp[2]++;
								break;
							} else if (validCompMove(board, comp[1]-1,comp[2]-1)){
								board[comp[1]-1][comp[2]-1] = 'O';
								comp[1]--;
								comp[2]--;
								break;
							} else if (validCompMove(board,comp[3]-1, comp[4]-1)){
								board[comp[3]-1][comp[4]] = 'O';
								comp[1] = comp[3]-1;
								comp[2] = comp[4]-1;
								break;
							} else if (validCompMove(board,comp[3]+1, comp[4]+1)){
								board[comp[3]+1][comp[4]] = 'O';
								comp[1] = comp[3]+1;
								comp[2] = comp[4]+1;
								break;
							} else { // New strat needed.
								firstComp(board, comp);
								comp[0] = -1;
								comp[5] = -1;
								break;						
							}
						} else {
							int move = n.nextInt(2);
							if (move == 0 && validCompMove(board,comp[1]-1,comp[2]+1)){ // attempt to place above 
								board[comp[1]-1][comp[2]+1] = 'O';
								comp[1]--;
								comp[2]++;
								break;
							} else if (validCompMove(board,comp[1]+1,comp[2]-1)){ // attempt to place below
								board[comp[1]+1][comp[2]-1] = 'O';
								comp[1]++;
								comp[2]--;
								break;
							} else if (validCompMove(board, comp[1]-1,comp[2]+1)){
								board[comp[1]-1][comp[2]+1] = 'O';
								comp[1]--;
								comp[2]++;
								break;
							} else if (validCompMove(board,comp[3]-1, comp[4]+1)){
								board[comp[3]-1][comp[4]+1] = 'O';
								comp[1] = comp[3]-1;
								comp[2] = comp[4]+1;
								break;
							} else if (validCompMove(board,comp[3]+1, comp[4]-1)){
								board[comp[3]+1][comp[4]-1] = 'O';
								comp[1] = comp[3]+1;
								comp[2] = comp[4]-1;
								break;
							} else { // New strat needed.
								firstComp(board, comp);
								comp[0] = -1;
								comp[5] = -1;
								break;						
							}
						}
					} else { // Computer doesn't own the middle of board
						if (validCompMove(board,2,2)){
							board[2][2] = 'O';
							comp[1] = 2;
							comp[2] = 2;
							break;
						} else { // Middle of board isn't avaliable and comp doesn't own -> get new strat
							comp[0] = -1;
							continue;
						}
					}
                }
			}
		/* there are two in a row, computer defaults to use an item to attempt to stop the player */
		} else if (danger[0] == 2) { //----------------------------------------------------------------------------------------------------------------
            System.out.println("-------------------------------2 in a row detected! NOT WORKING PRESENTLY AUG 21 2015---------------------------------" );
			compItem(board,points);
			scanCompBoard(board, danger, 'O'); // Check to see if the danger is still present            
            if (danger[0] == 2) { // Danger still exists must manually block the player
                if(danger[2] == 1) { // Horizontal block 
                    int prev = 0; 
                    for (int i = 0; i < board.length; i++) {
                        if (board[danger[1]][i] == 'X') {
                            break;
                        } else {
                            prev = i; 
                        }
                    }
                    // Players markers start at the first position in the matrix, offset 
                    if (prev == 0) {
                        prev += danger[0];
                    }
                    // attempt to place marker at the previous position on the board from the players
                    if (board[danger[1]][prev] != 'X' || board[danger[1]][prev] != 'D'){
                        board[danger[1]][prev] = 'O';
                    }
                } else if(danger[2] == 2) { // Vertical block! 
                    int prev = 0;
                    for (int i = 0; i < board.length; i++) {
                        if (board[i][danger[1]] == 'X'){
                            break;
                        } else {
                            prev = i;
                        }
                    }
                    // Players marker starts at the top of the matrix, offset to put in next position
                    if (prev == 0) {
                        prev += danger[0];
                    }
                    // Attempt to block the player at found locations
                    if (board[prev][danger[1]] != 'X' || board[prev][danger[1]] != 'D'){
                        board[prev][danger[1]] = 'O';
                    }
                } else { // danger must be horizontal, have to find it 
                    System.out.println("got here");
                }   
                
            }
        /* there are at least 3 X's in a row, computer must block the player in order to prevent a win */
		} else if (danger[0] > 3) { 
            System.out.println("-----three or more in a row!-------");
			int start = 0;
			danger[0]--;
			if (danger[3] == 1) { // 3 in a row is horizontal 
				for (int i = 0; i < board.length; i++ ) {
					if (board[danger[1]][i] == 'X') {
						start = i; 
						break;
					} 
				}
				if (start == 0) { // X's start at left hand edge of board
						board[danger[2]][start + danger[0]+1] = 'O'; // move made
						return;
				} else if (start+danger[0] == board.length-1) { // X's are at right hand edge of board
						board[danger[2]][start-1] = 'O'; // move made
						return;
				} else {
					Random r = new Random();
					int rn = r.nextInt(2);
					if (rn == 0) {
						board[danger[2]][start-1] = 'O'; // move made
						return;
					} else {
						board[danger[2]][start+danger[0]+1] = 'O';
						return;
					}
				}
			} else if (danger[3] == 2) { // Verticle 
				for(int i = 0; i < board.length; i ++) {
					if (board[i][danger[2]] == 'X') {
						start = i;
						break;
					}
				}
				if (start == 0) { // X's start at left hand edge of board
						board[start+danger[0]+1][danger[2]] = 'O'; // move made
						return;
				} else if (start+danger[0] == board.length-1) { // X's are at right hand edge of board
						board[start-1][danger[2]] = 'O'; // move made
						return;
				} else {
					Random r = new Random();
					int rn = r.nextInt(2);
					if (rn == 0) {
						board[start-1][danger[2]] = 'O'; // move made
						return;
					} else {
						board[start + danger[0]+1][danger[2]] = 'O';
						return;
					}
				}
			} else if (danger[3] == 3) { // Diagonals ********
				if (danger[2] != board.length-1) {
					for(int i = 0; i < board.length; i ++) {
						if (board[i][i] == 'X') {
							start = i;
							break;
						}
					}
				} else {
					for(int i = board.length-1; i >= 0; i--) {
						if (board[i][i] == 'X') {
							start = i;
							break;
						}
					}
				}
				if (start == 0) { // X's start at left hand edge of board
					board[start+danger[0]+1][start + danger[0]+1] = 'O'; // move made
					return;
				} else if (start+danger[0] == board.length-1) { // X's are at right hand edge of board
						board[board.length-1 - danger[0]-1][board.length-1 - danger[0]-1] = 'O'; // move made
						return;
				} else {
						Random r = new Random();
					int rn = r.nextInt(2);
					if (rn == 0) {
						board[start + danger[0] - 1][start + danger[0] - 1] = 'O'; // move made
						return;
					} else {
						board[start + danger[0] + 1][start + danger[0] + 1] = 'O';
						return;
					}
				}	
			}
		}
	}
	/* Scan's the COMPUTERS board to find out where the next character should be put.
		input - board, danger[3] and O
		output - values are returned in danger array
	*/
    /* ------------------------------------------------------------------------------------------------------------------*/
	public static void scanCompBoard(char[][] board, int[] danger, char xY) {
		int inRow = 0; // more trackers
		int indiag = 0; 
		int inVert = 0;
		int Hx = 0, Vy = 0, Dx = 0; // number in a row trackers 
		// checks for horizontals
		int compare =0; 
			for (int i = 0; i < board.length; i++) {
					if ( board[i][0] == xY) {
						for (int j = 0; j < board[i].length; j++) {
							if (board[i][j] == xY) {
								compare++; 
							} else { 
								break;
							}
						}
					}
			if (compare > inRow) {
				inRow = compare;
				Hx = i;
			}
			compare = 0;
		}
		// Scan for number of diagonals
		int diag = 0; 
		for (int i = 0; i < board.length; i++ ) {
				if (board [i][diag] == xY) {
						indiag++;
					}
				else {
					indiag = 0;
				}
					diag++;
		}
		Dx = 0;
		diag = board.length-1; 
		int temp = 0;
		for (int i = 0; i < board.length; i++) {
				if (board [i][diag] == xY ) {
						temp++; 
				} else {
					temp = 0;
				}
					diag--;
		}
		if (temp > indiag) {
			indiag = temp; 
			Dx = board.length-1;
		}
		// check for vertical wins
		int Tvert = 0;
		for (int i = 0; i < board.length; i++) {
				if (board[0][i] == xY) {
					for (int j= 0; j < board.length; j++ ) {
						if (board[j][i] == xY) {
							Tvert++;
						} else {
							break;
						} 
					}
				}
			if (Tvert > inVert) {
				inVert = Tvert;
				Vy = i;
			}
			Tvert = 0;
		}

		/* return the results based on which has the most in a row */
		if (inRow > indiag && inRow > inVert) {
			danger[0] = inRow;
			danger[1] = Hx;
			danger[2] = 1;  
		} else if (indiag > inRow && indiag > inVert) {
			danger[0] = indiag;
			danger[1] = Dx;
			danger[2] = 3;
		} else if (inVert > inRow && inVert > indiag) { 
			danger[0] = inVert;
			danger[1] = Vy;
			danger[2] = 2;
		} else if (inRow > indiag && inRow == inVert) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			}
		} else if ( inRow > inVert && inRow == indiag) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		} else if (inVert > inRow && inVert == indiag) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		} else if (inVert > indiag && inVert == inRow) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			}
		} else if (indiag > inRow && indiag == inVert) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			} else {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			}
		} else if (indiag > inVert && indiag == inRow) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			} else {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			}
		} else { // All in a row is equal return no danger, 
			Random rn = new Random();
			int random = rn.nextInt(2);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else if (random == 1) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		}
	}
	/* Scan board - Scans the board for number of characters in a row
					calculates which row/colum contains the most and 
					which type it is stored in array danger
		Input: Board, Danger[3] and char xY for which character to search for
		Output: results of scan are returned in Danger[3] respectively
	*/
	public static void scanBoard(char[][] board, int[] danger, char xY) {
		int inRow = 0; // Number of Char in a row
		int indiag = 0; // number of diagnoals in a row
		int inVert = 0; // number of verticles in a row
		int Hx = 0, Vy = 0, Dx = 0; // row/colm location trackers 
		
		// checks for horizontals --------------
		int compare =0; 
		int row = 0;
		for (int i = 0; i < board.length; i++) {
			for (int j = 0; j < board[i].length; j++) {
				if (board[i][j] == xY || board[i][j] == 'D') {
					compare++; 
				} else { 
					if (compare > row) {
						row = compare;
					}
					compare = 0;
				}
			}
			if (row > inRow) {
				inRow = row;
				Hx = i;
			}
			compare = 0;
		}
		// Scan for number of diagonals ------- 
		for (int i = 0; i < board.length; i++ ) {
			if (board [i][i] == xY || board[i][i] == 'D') {
					compare++;
			} else {
					if (compare > indiag) {
						indiag = compare;
					}
				compare = 0;
			}
		}
		Dx = 0;
		compare = 0;
		int rDiag = 0;
		// Both diag's are not working as of feb 12th
		for (int i = board.length-1; i <= 0; i--) {
				if (board[i][i] == xY || board[i][i] == 'D') {
						compare++; 
				} else {
					if (compare > rDiag) {
						rDiag = compare;
					}
					compare = 0;
				}
		}
		// choose which is greater between 2 diagonals
		if (rDiag > indiag) {
			indiag = rDiag; 
			Dx = board.length-1;
		}
		// check for vertical wins ----------------- 
		int Tvert = 0;
		int vert = 0;
		for (int i = 0; i < board.length; i++) {
			for (int j= 0; j < board.length; j++ ) {
				if (board[j][i] == xY || board[j][i] == 'D') {
					Tvert++; 
				} else {
					if ( Tvert > vert ) { 
						vert = Tvert;
					}
					Tvert = 0;
				} 
			}
			if (vert > inVert) {
				inVert = vert;
				Vy = i;
			}
			Tvert = 0;
		}
		// begins sections deciding which has more in a row.
		if (inRow > indiag && inRow > inVert) {
			danger[0] = inRow;
			danger[1] = Hx;
			danger[2] = 1;  
		} else if (indiag > inRow && indiag > inVert) {
			danger[0] = indiag;
			danger[1] = Dx;
			danger[2] = 3;
		} else if (inVert > inRow && inVert > indiag) { 
			danger[0] = inVert;
			danger[1] = Vy;
			danger[2] = 2;
		} else if (inRow > indiag && inRow == inVert) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			}
		} else if ( inRow > inVert && inRow == indiag) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		} else if (inVert > inRow && inVert == indiag) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		} else if (inVert > indiag && inVert == inRow) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			}
		} else if (indiag > inRow && indiag == inVert) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			} else {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			}
		} else if (indiag > inVert && indiag == inRow) {
			Random rn = new Random();
			int random = rn.nextInt(1);
			if (random == 0) {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			} else {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			}
		} else {
			Random rn = new Random();
			int random = rn.nextInt(2);
			if (random == 0) {
				danger[0] = inRow;
				danger[1] = Hx;
				danger[2] = 1;
			} else if (random == 1) {
				danger[0] = inVert;
				danger[1] = Vy;
				danger[2] = 2;
			} else {
				danger[0] = indiag;
				danger[1] = Dx;
				danger[2] = 3;
			}
		}
	}
	public static boolean checkForWin(int[] points, char[][] board, int player) {
		// Begin checking for horizontal win for either player
		for (int i = 0; i < board.length; i++) {
				if (player == 1) { 
					if ( board[i][0] == 'X') {
						for (int j = 0; j < board[i].length; j++) {
							if (board[i][j] != 'X') {
								break; // no win 
							} else if(j == board.length-1) {
								return true; // win found
							} else if (j > 1) {
								points[0] += 50; 
							}
						}
					}
				} else {
					if ( board[i][0] == 'O') {
						for (int j = 0; j < board[i].length; j++) {
							if (board[i][j] != 'O') {
								break; // no win 
							} else if( j == board.length-1) {
								return true; // win found
							} else if ( j > 1) {
								points[0] += 50; 
							}
						}
					}
				}
		}
		
		// check for diagonal wins 
		int diag = 0; 
		for (int i = 0; i < board.length; i++ ) {
			if (player == 1) {
				if (board [i][diag] != 'X' ) {
					break;
				} else {
					if (diag > 0){
						points[0] += 50;
					}
					diag++;
				}
				if ( i == board.length-1 ) {
					return true; 
				} 
			} else {
				if (board [i][diag] != 'O' ) {
					break;
				} else {
					if (diag > 0){
						points[0] += 50;
					}
					diag++;
				}
				if ( i == board.length-1 ) {
					return true; 
				} 
			}
				
		}
		diag = board.length-1; 
		for (int i = 0; i < board.length; i++) {
			if (player == 1) {
				if (board [i][diag] != 'X' ) {
					break;
				} else {
					if (diag < board.length-1) {
						points[0] += 50;
					}
					diag--;
				}
				if ( i == board.length-1 ) {
					return true; 
				} 
			} else {
				if (board [i][diag] != 'O' ) {
					break;
				} else {
					if (diag < board.length-1) {
						points[0] += 50;
					}
					diag--;
				}
				if ( i == board.length-1 ) {
					return true; 
				} 
			}
		}
		// check for vertical wins
		for (int i = 0; i < board.length; i++) {
			if (player == 1) {
				if (board[0][i] == 'X') {
					for (int j= 0; j < board.length; j++ ) {
						if (board[j][i] != 'X') {
							break; 
						} else if ( j == board.length-1) {
							return true;
						} else if (j > 1) {
							points[0] += 50;
						} 
					}
				}
			} else {
				if (board[0][i] == 'O') {
					for (int j= 0; j < board.length; j++ ) {
						if (board[j][i] != 'O') {
							break; 
						} else if ( j == board.length-1) {
							return true;
						} else if (j > 1) {
							points[0] += 50;
						} 
					}
				}
			}
		}
		
		return false;
		
		
	}
	public static boolean validCompMove(char[][] board,int x,int y) {
		if (x < 0 || y < 0 || x > 4 || y > 4) { // Array bounds check
			return false;
		}
		if (board[x][y] != ' ' && board[x][y] != 't' && board[x][y] != 'T') {
			return false;
		}
		return true;
	}
	
	public static boolean validMove(char[][] board,int x,int y) {
		if (x-1 < 0 || y-1 < 0) { // Array bounds check
			return false;
		}
		if (board[x-1][y-1] != ' ' && board[x-1][y-1] != 't' && board[x-1][y-1] != 'T') {
			return false;
		}
		return true;
	}
	
	public static void printboard(char[][] board) {
		System.out.println("----------------------------------------------------");
		System.out.print("\t");
		for (int i = 0; i < board.length; i++) { 
			System.out.print((i+1) + "   ");
		}
		System.out.println();
		for (int i =0; i < board.length; i++) {
				System.out.print((i+1) +"\t");
			for (int j = 0; j < board.length; j++){
				if (j != board.length-1) {
					System.out.print(board[i][j] + " | " );
				} else {
					System.out.print(board[i][j]);
				}
			}
			System.out.println();
		}
		System.out.println();
	}
	
	public static void printPlayer(char[][] board) {
		System.out.println("----------------------------------------------------");
		System.out.print("\t");
		for (int i = 0; i < board.length; i++) { 
			System.out.print((i+1) + "   ");
		}
		System.out.println();
		for (int i =0; i < board.length; i++) {
				System.out.print((i+1) +"\t");
			for (int j = 0; j < board.length; j++){
				if (j != board.length-1) {
					if (board[i][j] == 'X' || board[i][j] == 'T' || board[i][j] == 'D' || board[i][j] == 'O') {
						System.out.print(board[i][j] + " | " );
					} else if (board[i][j] == 'd') {
						System.out.print("O" + " | ");
					} else {
						System.out.print(" " + " | ");
					}
				} else {
					if (board[i][j] == 'X' || board[i][j] == 'T' || board[i][j] == 'D' || board[i][j] == 'O') {
						System.out.print(board[i][j]);
					} else if (board[i][j] == 'd') {
						System.out.print("O");
					} else {
						System.out.print(" ");
					}
				}
			}
			System.out.println();
		}
		System.out.println();
	}
	
	public static void printComp(char[][] board) {
		System.out.println("----------------------------------------------------");
		System.out.print("\t");
		for (int i = 0; i < board.length; i++) { 
			System.out.print((i+1) + "   ");
		}
		System.out.println();
		for (int i =0; i < board.length; i++) {
				System.out.print((i+1) +"\t");
			for (int j = 0; j < board.length; j++){
				if (j != board.length-1) {
					if (board[i][j] == 'O' || board[i][j] == 't' || board[i][j] == 'd' || board[i][j] == 'X') {
						System.out.print(board[i][j] + " | " );
					} else if (board[i][j] == 'D') {
						System.out.print("X" + " | ");
					} else {
						System.out.print(" " + " | ");
					}
				} else {
					if (board[i][j] == 'O' || board[i][j] == 't' || board[i][j] == 'd' || board[i][j] == 'X') {
						System.out.print(board[i][j]);
					} else if (board[i][j] == 'D') {
						System.out.print("X");
					} else {
						System.out.print(" ");
					}
				}
			}
			System.out.println();
		}
		System.out.println();
	}
		
	public static void abomb(char[][] board, Scanner input, int player) {
		int x = 0, y =0;
		System.out.println("Select a row and column to Nuke!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length);
		// set to destroy a perimeter of boardsize 
		int radius = 0;
		if (board.length == 5) {
			radius = 1; 
		} else {
			radius = 2;
		}
		x--; y--;
		// begin clearing here
		board[x][y] = ' ';
		// check for in bounds to reset spaces, x - horizontal, y - vertical
		for (int i = radius; i > 0; i--){
			if (!(y-i < 1)) {
				board[x][y-i] = ' ';
			}
			if (!(y+i > board.length)) {
				board[x][y+i] = ' ';
			}
			if (!(x-i < i)) {
				board[x-i][y] = ' ';
				if (!(y-i < i)) {
					board[x-i][y-i] = ' ';
				} 
				if (!(y+i > board.length)) {
					board[x-i][y+i] = ' ';
				}
			}
			if (!(x+i > board.length)) {
				board[x+i][y] = ' ';
				if (!(y-i < i)) {
					board[x+i][y-i] = ' ';
				} 
				if (!(y+i > board.length)) {
					board[x+i][y+i] = ' ' ;
				}
			}
		}
		if (player == 1) {
			printPlayer(board); 
		} else {
			printComp(board);
		}
	}
	
	public static boolean missile(char[][] board, Scanner input, int player) {
		int x = 0, y =0 ;
		System.out.println("Select a row and column to target!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length);
		if (board[x-1][y-1] == 'T' || board[x-1][y-1] == 't' || board[x-1][y-1] == 'O' || board[x-1][y-1] == 'X') {
			System.out.println("Target hit!");
			board[x-1][y-1] = ' ';
		} else if( board[x-1][y-1] == 'D' || board[x-1][y-1] == 'd') {
			System.out.println("Decoy hit!");
			board[x-1][y-1] = ' ';
			return false;
		} else {
			System.out.println("Miss!");
		}
		if (player == 1) {
			printPlayer(board);
		} else {
			printComp(board);
		}
		return true;
	}
	
	public static void trap(char[][] board, Scanner input, int player) {
		int x = 0, y =0 ;
		System.out.println("Select a row and column to place the trap!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x, y) == false) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x, y) == false);
		if (player == 1) {
			board[x-1][y-1] = 'T';
		} else {
			board[x-1][y-1] = 't';
		}
		System.out.println("Trap Set!");
		if (player == 1) {
			printPlayer(board);
		} else {
			printComp(board);
		}
	}
	
	public static void teleport(char[][] board, Scanner input, int player) {
		int x = 0, y =0 ;
		System.out.println("Select location for targeting!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length);
		int xx=0, yy=0;
		System.out.println("Select target to swap with:");
		do {
			xx = input.nextInt();
			yy = input.nextInt();
			if ( xx < 1 || yy < 1 || xx > board.length || yy > board.length ) {
				System.out.println("Invalid move! please choose another");
			}
		} while (xx < 1 || yy < 1 || xx > board.length || yy > board.length);
		x--; y--; xx--; yy--;
		System.out.println(board[x][y]);
		System.out.println(board[xx][yy]);
		char temp = board[x][y];
		board[x][y] = board[xx][yy];
		board[xx][yy] = temp;
		System.out.println("Teleport complete!\n\n");
		if (player ==1) {
			printPlayer(board);
		} else {
			printComp(board);
		}
		
	}
	
	public static void disarm(char[][] board, Scanner input, int player) {
		int x=0, y=0; 
		System.out.println("Select a row and column to attempt to disarm!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length);
		if (player == 1) {
			if (board[x-1][y-1] == 't') {
				board[x-1][y-1] = ' ';
				System.out.println("Disarm successful!");
			} else {
				System.out.println("No trap to disarm");
			}
		} else {
			if (board[x-1][y-1] == 'T') {
				board[x-1][y-1] = ' ';
				System.out.println("Disarm successful!");
			} else {
				System.out.println("No trap to disarm");
			}
		}
	}
	
	public static void decoy(char[][] board, Scanner input, int player) {
		int x = 0, y =0 ;
		System.out.println("Select a row and column to place the decoy!");
		do {
			x = input.nextInt();
			y = input.nextInt();
			if ( x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x, y) == false) {
				System.out.println("Invalid move! please choose another");
			}
		} while (x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x, y) == false);
		if (player == 1) {	
			board[x-1][y-1] = 'D';
		} else {
			board[x-1][y-1] = 'd';
		}
		System.out.println("Decoy Placed!");
	}
	
	public static void airStrike(char[][]board, Scanner Input, int player) {
		
		
		
	}
	public static boolean items(char[][] board, int[] points, int player) {
		Scanner input = new Scanner(System.in);
		int select = 0;
		int abomb = 800, missile = 300, trap = 200, teleport = 500, disarm = 50, decoy = 200; 
		boolean valid = true;
		System.out.println("Select an item - Current credits are: " + points[0]);
		System.out.println("1. Disarm - " + disarm + "  credits");
		System.out.println("2. Trap - " + trap + " credits" );
		System.out.println("3. Missile strike - " + missile + " credits");
		System.out.println("4. Decoy - " + decoy + " credits" );
		System.out.println("5. Teleport - " + teleport + " credits" );
		System.out.println("6. Nuke - " + abomb + " credits" );
		
		// valid input check
		
		do {
			select = input.nextInt();
			if (select < 1 || select > 6) {
				System.out.println("Invalid input, please choose an item from 1 to 6");
			}
			switch(select){
				case 6: 
					if ( points[0] - abomb < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
				case 3: 
					if (points[0] - missile < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
				case 2: 
					if ((points[0] - trap) < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
				case 5: 
					if ( points[0] - teleport < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
				case 1:
					if ( points[0] - disarm < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
				case 4: 
					if ( points[0] - decoy < 0) {
						System.out.println("Not enough credits! please select another item");
						valid = false;
					}
					break;
			}
		} while( select < 1 || select > 6 || valid == false);
		switch(select){
		case 6: 
			abomb(board,input, player);
			points[0] = points[0] - abomb;
			move(board, player);
			break;
		case 3:
			boolean stunned = missile(board,input, player);
			points[0] = points[0] - missile;
			if (stunned == false){
				return true; 
			}else {
				move(board, player);
				break;
			}
		case 2:
			trap(board,input, player);
			points[0] = points[0] - trap;
			move(board, player);
			break;
		case 5: 
			teleport(board,input, player);
			points[0] = points[0] - teleport;
			move(board, player);
			break;
		case 1: 
			disarm(board,input, player);
			points[0] = points[0] - disarm;
			move(board, player);
			break;
		case 4:
			decoy(board,input, player);
			points[0] = points[0] - decoy;
			break;
		}
		return false;
	}
	
	public static void move(char[][] board, int player) {
		Scanner input = new Scanner(System.in);
		int x, y; 
		System.out.println("Please select a row and column to mark" );
		do {
			x = input.nextInt();
			y = input.nextInt();
			if( x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x,y) == false){
				System.out.println("Invalid move, please select another move");
			}
		} while ( x < 1 || y < 1 || x > board.length || y > board.length || validMove(board, x ,y) == false);
		
		
		if (board[x-1][y-1] == 'T' && player == 2 || board[x-1][y-1] == 't' && player == 1){
				System.out.println("Trap hit!");
				board[x-1][y-1] = ' ';
		} else {
			if (player == 1) {
				board[x-1][y-1] = 'X';
			} else 
				board[x-1][y-1] = 'O';
		}
		System.out.println("Marked! - End of player " + player + "'s turn\n\n");
	}
	
	public static void main(String[] args) {
		int[] points = {10000};
		int[] p2points = {10000};
		int player = 1; // tracks which players turn it is, player 1 is default
		int mode = 1; // 1 = versus player, 2 = versus computer
		boolean quit = false;
		/* 	comp[0] = strat, comp[1] = x, comp[2] = y, comp[3] = original x, 
			comp[4] = original y, comp[5] = diagonal strat type */
		int[] comp = {-1, 0, 0, 0, 0, 0};
		Scanner input = new Scanner(System.in);
		
		System.out.println("Welcome to Battletac! Please select a game type");
		System.out.println("1 - Normal");
		System.out.println("2 - Large");
		int selection = 0; 
		do {
			//try {
				selection = input.nextInt();
			/*} catch (inputMismatchException e) {
				System.out.println("Invalid input, please enter 1 or 2! " );
				continue;
			}*/
			if (selection != 1 && selection != 2) {
				System.out.println("invalid choice! Please enter 1 or 2! ");
			}
		} while (selection != 1 && selection != 2);
		int x = 0, y=0;
		if (selection == 1) { 
			x = 5;
			y = 5;
		} else if (selection == 2) {
			x = 7;
			y = 7;
		}
		System.out.println("To play Single player press 1 or press 2 for multiplayer");
		// initialization of game board
		char[][] board = new char[x][y];
		for (int i = 0; i < board.length; i++) 
			for (int j = 0; j < board[0].length; j++)
				board[i][j] = ' ';
		// -------------------------------------------------------
		do {
			//try {
				selection = input.nextInt();
			/*} catch (inputMismatchException e) {
				System.out.println("Invalid input, please enter 1 or 2! " );
				continue;
			}*/
			if (selection != 1 && selection != 2) {
				System.out.println("invalid choice! Please enter 1 or 2! ");
			}
		} while (selection != 1 && selection != 2);
		mode = selection;
		/* first PLAYER turn begins here */	
		System.out.println();
		System.out.println( "Player 1's turn --- " );
		printboard(board);
		move(board, player);
		points[0] = points[0] + 100;
		/* END PLAYER FIRST TURN ---------------------------
		COMPUTER OR PLAYER 2 TURN BEGINS HERE */
		player = 2;
		if (mode == 2) {	
			System.out.println();
			System.out.println( "Player 2's turn --- " );
			printboard(board);
			move(board, player);
			p2points[0] = p2points[0] + 100;
		} else {
			System.out.println();
			System.out.println( "Computer's turn --- " );
			firstComp(board,comp);
			p2points[0] = p2points[0] + 100;
			printboard(board);
		}
		boolean stunned1 = false;
		boolean stunned2 = false;
		// Begins game starting with player 1
		do {
			/*-------------------------- Player 1 STARTS HERE ---------------------------------------- */
			player = 1;
			if (stunned1) {
				System.out.println("Player 1 is Stunned!! \n\n" );
				stunned1 = false;
			} else {
				System.out.println( "Player 1's turn --- " );
				System.out.println(" Current Points = " + points[0]);
				printPlayer(board);
				System.out.println("Press 1 to mark a spot or 2 to use an item");
				do {
					selection = input.nextInt();
					if (selection != 1 && selection != 2) {
						System.out.println("invalid choice, select 1 or 2! ");
					}
				} while (selection != 1 && selection != 2);
				
				if (selection == 2) {
					stunned1 = items(board, points, player);
				} else if (selection == 1) {
					move(board, player);	
				}
				points[0] = points[0] + 100;
				if (checkForWin(points, board, player) == true) {
					printboard(board);
					System.out.println("Player " + player + " wins!!!");
					return;
				}
                printPlayer(board);
			}
			/*----------------------------------------------------------------------------------------------------*/
			/* ---------------------------PLAYER 2 STARTS HERE ---------------------------------*/
			if (mode == 2) { // 2 is playing with a computer
				if (stunned2) {
					System.out.println(" Player 2 is Stunned!!\n\n" );
					stunned2 = false;
				} else {
					System.out.println( "Player 2's turn --- " );
					System.out.println(" Current Points = " + p2points[0]);
					player = 2; // end of player 1 turn
					printComp(board);
					System.out.println("Press 1 to mark a spot or 2 to use an item");
					do {
						selection = input.nextInt();
						if (selection != 1 && selection != 2) {
							System.out.println("invalid choice, select 1 or 2! ");
						}
					} while (selection != 1 && selection != 2);
					
					if (selection == 2) {
						stunned2 = items(board, points, player);
					} else if (selection == 1) {
						move(board, player);	
					}
					p2points[0] = p2points[0] + 100;
					if (checkForWin(p2points, board, player) == true) {
						printboard(board);
						System.out.println("Player " + player + " wins!!!");
						return;
					}
				}
				// -------------------------- Computer selection begins here --------------------------------
			} else {
				if (stunned2) {
					System.out.println(" Computer is Stunned!!\n\n" );
					stunned2 = false;
				} else {
					System.out.println( "Computer's turn --- " );
					player = 2; // end of player 1 turn
					p2points[0] = p2points[0] + 100;
					comp(p2points, board, comp);
					if (checkForWin(p2points, board, player) == true) {
						printboard(board);
						System.out.println("Computer wins!!!! ");
						return;
					}
				}
			}
			/*---------------------------------------------------------------------------------------------------- */
		} while(true);
	}
}