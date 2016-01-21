<<<<<<< HEAD
/*****************************************************
	Canadian Life simulator 
	Created By Brady Schnell
	Project start date Dec 04 2015
	Last update: Dec 07 2015
	
	This game is designed to demonstrate the level of discrimination that
	canadians of different upbringings face on a daily level. 
	the player wins the game if they manage to get the best house, a high end job,
	and a partner.
******************************************************/
#include <string>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <windows.h>

using namespace std;

class Player {
	private:
		string race; // Tracks the players Race
		int gender; // Players Gender
		string gender_name; // Players gender in string
		int sex_num; // int representing type of sexual orientation
		string sex_type; // string for sexual orientation

		float wage; // Players wages 
		float living_cost; // amount money the player will pay.
		float savings;  // Savings player currently has
		float discrim; // this will be a value between 0 and 1
		bool has_partner; // Sets whether the player has a partner which doubles your earnings but 75% increase to billings
		bool has_job;
		bool has_house;
		int food; // Players food level
		int house_size; // tracks the size of the house. Players goal is to get the biggest house. 1 - smallest
		int job_level;

	public:
		void init() { 
			wage = 0.0f;
			has_partner = false;
			savings = 0.0f;
			discrim = 0.0f;
			food = 10;
			house_size = 0; 
			has_job = false;
			living_cost = 0;			
		}

		/* Setters start here */
		void setRace(string race) {
			this->race = race;
		}

		void setGender(int sex, string Gender) {
			this->gender = sex;
			this->gender_name = Gender;
		}

		void setWage(float salary) {
			this->wage = salary;
		}

		void setSex(int num, string type) {
			this->sex_num = num;
			this->sex_type = type;
		}

		void setHouse(int size) {
			this->house_size = size;
		}

		void setJobStatus(bool status) {
			this->has_job = status;
		}

		void setJobLevel(int lev) {
			this->job_level = lev;
		}

		void setPartner(bool hasPartner) {
			this->has_partner = hasPartner;
		}

		void setHasHouse(bool hasHouse) {
			this->has_house = hasHouse;
		}

		void setLivingCost(float amount) {
			this->living_cost = amount;
		}

		/* getters start here */
		string getRace() {
			return race;
		}

		string getGenderName() {
			return gender_name;
		}

		float getWage() {
			return wage;
		}

		string getSexuaL() {
			return sex_type;
		}

		string gethouse() {
			if (house_size == 1 ) {
				return "Small house";
			} else if (house_size == 2) {
				return "Medium house";
			} else if (house_size == 3) {
				return "Large house!";
			} else {
				return "Homeless";
			}
		}

		string getJoblevel() {
			if (job_level == 3 ) {
				return "High";
			} else if (job_level == 2) {
				return "Medium";
			} else if (job_level == 1) {
				return "Low";
			} else {
				return "Unemployed :(";
			}
		}

		int getFoodLevel() {
			return food;
		}

		float getSavings() {
			return savings;
		}

		float getBills() {
			return living_cost;
		}

		/* has functions */
		bool hasjob() {
			return has_job;
		}

		bool hasHouse() {
			return has_house;
		}

		bool hasPartner() {
			return has_partner;
		}

		/* modifers start here */
		void addCosts(float value) {
			living_cost += value;
		}

		void subtractMoney(float value) {
			savings -= value; 
		}

		void addMoney() {
			savings += wage;
		}

		void addGroceries() {
			food += 8;
		}
		void removeFood() {
			food -= 2;
		}

		void subtractGroceries() {
			savings -= 500;
		}

		void payBills() {
			savings -= living_cost;
		}
};

Player* generatehouse(Player* player) {
	for(;;) {
		cout << "Attempting to buy house..." << endl;
		Sleep(5000);

		int rnd = rand() % 10 + 1; 
		if (rnd <= 3) { // 30% chance to be able to get a large house. 
			if (player->getRace() != "Indigenous" && player->getRace() != "Black" && player->getGenderName() != "Other") { // if the player isn't white, they can't get it.
				if (player->hasPartner()) {
					player->setLivingCost(1500 * 1.5);
				} else {
					player->setLivingCost(1500); // add monthly house cost. 
				}
				player->setHouse(3); // Player owns b
				player->setHasHouse(true);
				break;
			}
		}

		rnd = rand() % 10 + 1; 
		if (rnd <= 6) { // 60% chance to buy a medium house if not black or indigenous
			if (player->getRace() != "Indigenous" && player->getRace() != "Black" && player->getGenderName() != "Other") {
				if (player->hasPartner()) {
					player->setLivingCost(1250 * 1.5);
				} else {
					player->setLivingCost(1250); // add monthly house cost. 
				}
				
				player->setHouse(2); 
				player->setHasHouse(true);
				break;
			}
		}

		if (player->getRace() == "Indigenous" || player->getRace() == "Black") {
			if (rnd <= 3) {
				if (player->hasPartner()) {
					player->setLivingCost(1250 * 1.5);
				} else {
					player->setLivingCost(1250); // add monthly house cost. 
				}
				player->setHouse(2); 
				player->setHasHouse(true);
				break;
			}
		}

		rnd = rand() % 10 + 1; 
		if (rnd <= 9) { // 90% chance to buy the smallest house.
			if (player->hasPartner()) {
					player->setLivingCost(1000 * 1.5);
				} else {
					player->setLivingCost(1000); // add monthly house cost. 
				}
			player->setHouse(1);
			player->setHasHouse(true);
			break;
		} 

		if (rnd == 10) {
			player->setLivingCost(0);
			player->setHouse(0);
			player->setHasHouse(false);
			break;
		}

		if (player->hasHouse() == true) {
			break;
		}
	}

	return player;
}

Player* startjob(Player* player) { 
	string prevjob = player->getJoblevel();
	// if the player already has a job, it will only try once.
	for(;;) {
		cout << "Applying for a new job!..." << endl;
		Sleep(5000);
		int rnd = rand() % 10 + 1; 
		if (rnd <= 3 && player->getRace() != "Black" && player->getRace() != "Indigenous" && player->getGenderName() != "Other") { // everyone has a 30% chance to become rich...
			if (player->hasPartner()) {
				player->setWage((100000/24) * 1.75);
			} else {
				player->setWage(100000/24); // salary comes from 
			}
			player->setJobStatus(true); // player now has a job
			player->setJobLevel(3);
			break;
		}
		
		rnd = rand() % 10 + 1;
		if (rnd <= 5) { // 50% chance to earn a decent income
			if (player->hasPartner()) {
				player->setWage((40000/24)* 1.75);
			} else {
				player->setWage(40000/24);
			}
			player->setJobStatus(true);
			player->setJobLevel(2);
			break;
		} 

		rnd = rand() % 10 + 1;
		if (rnd <= 9 ) {// 90% chance to get a low paying job
			if (player->hasPartner()) {
				player->setWage((25000/24) * 1.75);
			} else {
				player->setWage(25000/24);
			}
			player->setJobStatus(true); 
			player->setJobLevel(1);
			break;
		}

		if (rnd == 10) {
			player->setWage(0);
			player->setJobStatus(true);
			player->setJobLevel(0);
			cout << "You are now unemployed!" << endl;
			break;
		}

		if (player->hasjob() == true) {
			break;
		}
	}
	
	string newjob = player->getJoblevel();
	/* if the job changed do the female tax again */
	if (newjob.compare(prevjob) != 0) {
		/* the female tax. they can only earn 78% of the income of men */
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other") {
			cout << "oh no! because you we are a woman you are only able to earn 78% of the wage!" << endl;
			Sleep(2000);
			player->setWage(player->getWage() * 0.78);
		} 
	}

	return player;
}

void FindPartner(Player* player) {
	int rnd = rand() % 100 + 1; 
	player->setPartner(false);
	cout << "looking for a partner!" << endl;
	Sleep(4000);
	if (player->getSexuaL() == "Other") {
		if (rnd <= 20) { 
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} else if (player->getSexuaL() == "Homosexual") {
		if (rnd <= 30) {
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} else if (player->getSexuaL() == "Heterosexual") {
		if (rnd <= 40) {
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} 

	if (player->hasPartner() == true) {
		cout << "You found a partner!" << endl;
	} else { 
		cout << "You were unable to find a new partner :(" << endl;
	}
}

void calculateWage(Player* player) {
	player->setPartner(false);
	if (player->getJoblevel() == "Low") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((25000/24) * .78);
		} else {
			player->setWage(25000/24);
		}
	} else if (player->getJoblevel() == "Medium") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((40000/24) * .78);
		} else {
			player->setWage(40000/24);
		}
	} else if (player->getJoblevel() == "High") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((100000/24) * .78);
		} else {
			player->setWage(100000/24);
		}
	}

	if (player->gethouse() == "Small house") {
		player->setLivingCost(1000);
	} else if (player->gethouse() == "Medium house") {
		player->setLivingCost(1250);
	} else if (player->gethouse() == "Large house!") {
		player->setLivingCost(1500);
	}

}

void print_information(Player* player) {
	cout << endl << "------------Players stats-----------" << endl;
	cout << "Food level: " << player->getFoodLevel() << endl;
	cout << "Wage level: " + player->getJoblevel() << endl;
	cout << "Savings: " << player->getSavings() << endl;
	cout << "House Size: " + player->gethouse() << endl;
	if (player->hasPartner() == true) {
		cout << "Relationship status: In a relationship" << endl;
	} else {
		cout << "Relationship status: Single" << endl;
	}
	cout << "------------------------------------" << endl << endl;
}

int main () {
	Player* player = new Player();
	player->init();
	cout << "Welcome to the Canadian Life Simulator!" << endl; 
	Sleep(2000); // 1 sec == 1000 ms, so sleep for 2 seconds
	cout << "You will be spawned into the world shortly!" << endl;
	Sleep(3000);
	cout << "Just like the real world, you will be assigned completely random characteristics!" << endl;
	Sleep(3000);
	cout << "Lets begin by first determining your ethnicity." << endl;
	Sleep(5000);
	srand (time(NULL));

	/* Generation of the players race will be done now */
	string race[5] = {"White", "Asian", "Black", "Indigenous", "Indian (east)" };

	int rnd = rand() % 5; // Generates random number between 0 and 4

	player->setRace(race[rnd]);

	cout << "You were born of " + race[rnd] + " ethnicity!" << endl << endl;
	Sleep(4000);
	/* this value will be between 1 and 3. 1 - male, 2 - female, 3 - other */
	cout << "Next, we will determine your gender." << endl;
	Sleep(4000);
	int gender = 0; 
	string sextype[3] = {"Heterosexual", "Homosexual", "Other"};

	/* Generates a random number between 0 and 2 for races */
	gender = rand() % 10 + 1;

	if (gender <= 4 ) { // This will be for a male 
		player->setGender(0, "Male");
	} else if (gender <= 8) { // This will be female
		player->setGender(1, "Female");
	} else if (gender >= 9) {
		player->setGender(2, "Other");
	}

	cout << "Your gender is " + player->getGenderName() << "." << endl << endl;
	Sleep(4000);
	/* this section will generate the players sexual orientation */

	cout << "Finally, we will determine your sexual orientation." << endl;
	Sleep(4000);
	rnd = rand() % 3; // Generate a random number between 0 and 2.

	player->setSex(rnd, sextype[rnd]);

	cout << "Your sexual orientation is: " + player->getSexuaL() << "." << endl << endl;
	Sleep(4000);
	bool game = true;
	
	cout << endl;
	cout << "Now you will begin to live your life." << endl;
	Sleep(4000);
	cout << "Each turn you can pursue a different aspect of life." << endl;
	Sleep(4000);
	cout << "Your characters traits affect your chances to achieve different things." << endl << endl;
	Sleep(4000);
	cout << "The objective of the game is to get a large house, a high paying job, and be in a relationship." << endl << endl;
	Sleep(5000);
	cout << "Next, we need to get you somewhere to live." << endl;
	Sleep(4000);
	cout << "Your race effects the type of house you are able to get." << endl;
	Sleep(4000);
	cout << "Buying your new house now!" << endl << endl;
	Sleep(1000);
	player = generatehouse(player); 
	cout << "You were able to purchase a " + player->gethouse() << " congratulations!" << endl << endl;
	Sleep(4000);
	int billing = 5; // counts down from 5 each turn, at 1 player is billed their billing amount
	

	cout << "Now we need to get you a job!" << endl;
	Sleep(3000);
	player = startjob(player);
	cout << "You started work at a " + player->getJoblevel() + " paying job." << endl;
	cout << endl;

	/* more intro stuff here */
	cout << "Every two turns you be paid a wage, which you can use to purchase various things!" << endl;
	Sleep(4000);
	cout << "If you run out of money you will lose the game." << endl << endl;
	Sleep(4000);
	cout << "Your character also has a food level that starts at 10." << endl;
	Sleep(4000);
	cout << "Each turn, your food level will drop by 2." << endl;
	Sleep(4000);
	cout << "If your food level hits zero, you will lose the game." << endl;
	Sleep(4000);
	cout << "You can replenish your food level by purchasing groceries for $500." << endl << endl;
	Sleep(4000);
	cout << "Lets get started!" << endl << endl;
	Sleep(3000);
	/* Beginning of main game loop now that the player has had their stats initalized. */
	// at the end of each turn the player will "go to work" so then they will get paid. 
	int actiondone = 1; // if this goes to zero the player will have no more moves.
	int turncounter = 1;
	int billcounter = 1;
	bool player_warned_job = false;
	bool player_warned_house = false;
	string input; 

	while (game) {
		if (billcounter == 5) {
			cout << "Paying bills..." << endl;
			Sleep(4000);
			player->payBills();
			billcounter = 1;
		}
		// This is the beginning of a players turn.
		if (player->getFoodLevel() < 0) {
			cout << "You have run out of food!..." << endl;
			Sleep(3000);
			cout << "Game over!" << endl << endl;
			Sleep(20000);
			return 0;
		}

		if (player->getSavings() < 0) {
			cout << "You have gone bankrupt!...." << endl;
			Sleep(3000);
			cout << "Game over!" << endl << endl;
			Sleep(20000);
			return 0;
		}

		if (player->hasPartner() == true && player->getJoblevel() == "High" && player->gethouse() == "Large house!") {
			cout << "Congradulations, you have won the game!" << endl;
			Sleep(20000);
			return 0;
		}

		if (turncounter == 2) {
			cout << "Payday!" << endl;
			player->addMoney();
			turncounter = 0;
		}

		if (player->hasPartner()) {
			rnd = rand() % 100 +1; // Generate a random number between 1-10 

			if (player->getSexuaL() == "Heterosexual") {
				if (rnd <= 30) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			} else if (player->getSexuaL() == "Homosexual") {
				if (rnd <= 35) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			} else if (player->getSexuaL() == "Other") {
				if (rnd <= 40) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			}

		}

		actiondone = 1;

		while (actiondone == 1) {
			// at the start of each turn print out the players information. 
			// Displays on screen all the characters stats
			cout << "Bills due in : " << (5-billcounter) << " turns." << endl;
			cout << "Payday in: " << (2-turncounter) << " turns." << endl;
			print_information(player);

			cout << "What would you like do?" << endl;
			cout << "1. Look for a relationship." << endl;
			cout << "2. Search for a new job." << endl;
			cout << "3. Look for a new house ($1000)." << endl;
			cout << "4. Buy groceries ($500)." << endl;
			cout << "5. display character info." << endl;
			cout << "6. Pass (use this if you don't want to do anything this turn)." << endl << endl;
			cout << "Enter a number between 1 and 6 then press enter (q to quit)." << endl;	
		
			getline (cin,input);

			while(input != "1" && input != "2" && input != "3" && input != "4" && input != "5" && input != "q" && input != "Q" && input != "6" && input != "showmethecowlevel") {
				cout << "Invalid input, please enter a number between 1 and 6." << endl;
				getline (cin,input);
			}

			if (input == "1") {
				FindPartner(player);
				actiondone--;
			} else if (input == "2") {
				if (player_warned_job == false) {
					cout << "Warning! Searching for a new job can result in you getting a worse job than before." << endl;
					cout << "Do you wish to continue? (enter y or n)" << endl;
					player_warned_job = true;
					getline (cin, input);
					while (input != "n" && input != "y") {
						cout << "invalid input, please enter y or n" << endl;
						getline(cin,input);
					}
					/* check players input, continue or reset if y or n. */
					if (input == "y") {
						player = startjob(player);
						cout << "You got a " + player->getJoblevel() + " paying job." << endl;
						Sleep(2000);
						actiondone--;
					} else {
						continue;
					}
				} else {
					player = startjob(player);
					cout << "You got a " + player->getJoblevel() + " paying job." << endl;
					Sleep(2000);
					actiondone--;
				}
			} else if (input == "3") {
				if (player_warned_house == false) {
					cout << "Warning! Searching for a new house can result in you getting a worse house than what you had before." << endl;
					cout << "Do you wish to continue? (enter y or n)." << endl;
					player_warned_house = true;
					getline (cin, input);
					while (input != "n" && input != "y") {
						cout << "invalid input, please enter y or n." << endl;
						getline(cin,input);
					}
					/* check players input, continue or reset if y or n. */
					if (input == "y") {
						if (player->getSavings() < 1000) {
							cout << "You cannot afford to buy a new house. It costs $1000." << endl;
							continue;
						} else {
							player->subtractMoney(1000);
							player = generatehouse(player);
							cout << "You were able to purchase a " + player->gethouse() << endl << endl;
							Sleep(2000);
							actiondone--;
						}
					} else {
						continue;
					}
				} else {
					if (player->getSavings() < 1000) {
						cout << "You cannot afford to buy a new house. It costs $1000." << endl;
						continue;
					} else {
						player->subtractMoney(1000);
						player = generatehouse(player);
						cout << "You were able to purchase a " + player->gethouse() << "." << endl << endl;
						Sleep(2000);
						actiondone--;
					}
				}
			} else if (input == "4") {
				if(player->getSavings() <= 500) {
					cout << "You cannot afford groceries. They cost $500." << endl;
				} else {
					cout << "Buying groceries..." << endl << endl;
					Sleep(3000);
					player->subtractGroceries();
					player->addGroceries();
					actiondone--;
				}
			} else if (input == "5") {
				print_information(player);
			} else if (input == "q" || input == "Q") {
				return 0;
			} else if (input == "6") {
				actiondone--;
			} else if (input == "showmethecowlevel") {
				player->setWage((100000/24) * 1.75);
				player->setJobStatus(true); // player now has a job
				player->setJobLevel(3);
				player->setHouse(3); // Player owns b
				player->setHasHouse(true);
				player->addCosts(1000);
				player->setPartner(true);
				actiondone--;
			}
		}

		cout << "End of turn, going to work..." << endl << endl;
		Sleep(3000);
		player->removeFood(); 
		turncounter++;
		billcounter++;
	}
	return 0; 
=======
/*****************************************************
	Canadian Life simulator 
	Created By Brady Schnell
	Project start date Dec 04 2015
	Last update: Dec 07 2015
	
	This game is designed to demonstrate the level of discrimination that
	canadians of different upbringings face on a daily level. 
	the player wins the game if they manage to get the best house, a high end job,
	and a partner.
******************************************************/
#include <string>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <windows.h>

using namespace std;

class Player {
	private:
		string race; // Tracks the players Race
		int gender; // Players Gender
		string gender_name; // Players gender in string
		int sex_num; // int representing type of sexual orientation
		string sex_type; // string for sexual orientation

		float wage; // Players wages 
		float living_cost; // amount money the player will pay.
		float savings;  // Savings player currently has
		float discrim; // this will be a value between 0 and 1
		bool has_partner; // Sets whether the player has a partner which doubles your earnings but 75% increase to billings
		bool has_job;
		bool has_house;
		int food; // Players food level
		int house_size; // tracks the size of the house. Players goal is to get the biggest house. 1 - smallest
		int job_level;

	public:
		void init() { 
			wage = 0.0f;
			has_partner = false;
			savings = 0.0f;
			discrim = 0.0f;
			food = 10;
			house_size = 0; 
			has_job = false;
			living_cost = 0;			
		}

		/* Setters start here */
		void setRace(string race) {
			this->race = race;
		}

		void setGender(int sex, string Gender) {
			this->gender = sex;
			this->gender_name = Gender;
		}

		void setWage(float salary) {
			this->wage = salary;
		}

		void setSex(int num, string type) {
			this->sex_num = num;
			this->sex_type = type;
		}

		void setHouse(int size) {
			this->house_size = size;
		}

		void setJobStatus(bool status) {
			this->has_job = status;
		}

		void setJobLevel(int lev) {
			this->job_level = lev;
		}

		void setPartner(bool hasPartner) {
			this->has_partner = hasPartner;
		}

		void setHasHouse(bool hasHouse) {
			this->has_house = hasHouse;
		}

		void setLivingCost(float amount) {
			this->living_cost = amount;
		}

		/* getters start here */
		string getRace() {
			return race;
		}

		string getGenderName() {
			return gender_name;
		}

		float getWage() {
			return wage;
		}

		string getSexuaL() {
			return sex_type;
		}

		string gethouse() {
			if (house_size == 1 ) {
				return "Small house";
			} else if (house_size == 2) {
				return "Medium house";
			} else if (house_size == 3) {
				return "Large house!";
			} else {
				return "Homeless";
			}
		}

		string getJoblevel() {
			if (job_level == 3 ) {
				return "High";
			} else if (job_level == 2) {
				return "Medium";
			} else if (job_level == 1) {
				return "Low";
			} else {
				return "Unemployed :(";
			}
		}

		int getFoodLevel() {
			return food;
		}

		float getSavings() {
			return savings;
		}

		float getBills() {
			return living_cost;
		}

		/* has functions */
		bool hasjob() {
			return has_job;
		}

		bool hasHouse() {
			return has_house;
		}

		bool hasPartner() {
			return has_partner;
		}

		/* modifers start here */
		void addCosts(float value) {
			living_cost += value;
		}

		void subtractMoney(float value) {
			savings -= value; 
		}

		void addMoney() {
			savings += wage;
		}

		void addGroceries() {
			food += 8;
		}
		void removeFood() {
			food -= 2;
		}

		void subtractGroceries() {
			savings -= 500;
		}

		void payBills() {
			savings -= living_cost;
		}
};

Player* generatehouse(Player* player) {
	for(;;) {
		cout << "Attempting to buy house..." << endl;
		Sleep(5000);

		int rnd = rand() % 10 + 1; 
		if (rnd <= 3) { // 30% chance to be able to get a large house. 
			if (player->getRace() != "Indigenous" && player->getRace() != "Black" && player->getGenderName() != "Other") { // if the player isn't white, they can't get it.
				if (player->hasPartner()) {
					player->setLivingCost(1500 * 1.5);
				} else {
					player->setLivingCost(1500); // add monthly house cost. 
				}
				player->setHouse(3); // Player owns b
				player->setHasHouse(true);
				break;
			}
		}

		rnd = rand() % 10 + 1; 
		if (rnd <= 6) { // 60% chance to buy a medium house if not black or indigenous
			if (player->getRace() != "Indigenous" && player->getRace() != "Black" && player->getGenderName() != "Other") {
				if (player->hasPartner()) {
					player->setLivingCost(1250 * 1.5);
				} else {
					player->setLivingCost(1250); // add monthly house cost. 
				}
				
				player->setHouse(2); 
				player->setHasHouse(true);
				break;
			}
		}

		if (player->getRace() == "Indigenous" || player->getRace() == "Black") {
			if (rnd <= 3) {
				if (player->hasPartner()) {
					player->setLivingCost(1250 * 1.5);
				} else {
					player->setLivingCost(1250); // add monthly house cost. 
				}
				player->setHouse(2); 
				player->setHasHouse(true);
				break;
			}
		}

		rnd = rand() % 10 + 1; 
		if (rnd <= 9) { // 90% chance to buy the smallest house.
			if (player->hasPartner()) {
					player->setLivingCost(1000 * 1.5);
				} else {
					player->setLivingCost(1000); // add monthly house cost. 
				}
			player->setHouse(1);
			player->setHasHouse(true);
			break;
		} 

		if (rnd == 10) {
			player->setLivingCost(0);
			player->setHouse(0);
			player->setHasHouse(false);
			break;
		}

		if (player->hasHouse() == true) {
			break;
		}
	}

	return player;
}

Player* startjob(Player* player) { 
	string prevjob = player->getJoblevel();
	// if the player already has a job, it will only try once.
	for(;;) {
		cout << "Applying for a new job!..." << endl;
		Sleep(5000);
		int rnd = rand() % 10 + 1; 
		if (rnd <= 3 && player->getRace() != "Black" && player->getRace() != "Indigenous" && player->getGenderName() != "Other") { // everyone has a 30% chance to become rich...
			if (player->hasPartner()) {
				player->setWage((100000/24) * 1.75);
			} else {
				player->setWage(100000/24); // salary comes from 
			}
			player->setJobStatus(true); // player now has a job
			player->setJobLevel(3);
			break;
		}
		
		rnd = rand() % 10 + 1;
		if (rnd <= 5) { // 50% chance to earn a decent income
			if (player->hasPartner()) {
				player->setWage((40000/24)* 1.75);
			} else {
				player->setWage(40000/24);
			}
			player->setJobStatus(true);
			player->setJobLevel(2);
			break;
		} 

		rnd = rand() % 10 + 1;
		if (rnd <= 9 ) {// 90% chance to get a low paying job
			if (player->hasPartner()) {
				player->setWage((25000/24) * 1.75);
			} else {
				player->setWage(25000/24);
			}
			player->setJobStatus(true); 
			player->setJobLevel(1);
			break;
		}

		if (rnd == 10) {
			player->setWage(0);
			player->setJobStatus(true);
			player->setJobLevel(0);
			cout << "You are now unemployed!" << endl;
			break;
		}

		if (player->hasjob() == true) {
			break;
		}
	}
	
	string newjob = player->getJoblevel();
	/* if the job changed do the female tax again */
	if (newjob.compare(prevjob) != 0) {
		/* the female tax. they can only earn 78% of the income of men */
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other") {
			cout << "oh no! because you we are a woman you are only able to earn 78% of the wage!" << endl;
			Sleep(2000);
			player->setWage(player->getWage() * 0.78);
		} 
	}

	return player;
}

void FindPartner(Player* player) {
	int rnd = rand() % 100 + 1; 
	player->setPartner(false);
	cout << "looking for a partner!" << endl;
	Sleep(4000);
	if (player->getSexuaL() == "Other") {
		if (rnd <= 20) { 
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} else if (player->getSexuaL() == "Homosexual") {
		if (rnd <= 30) {
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} else if (player->getSexuaL() == "Heterosexual") {
		if (rnd <= 40) {
			player->setPartner(true);
			player->setWage(player->getWage() * 1.75);
			player->setLivingCost(player->getBills() * 1.50);
		}
	} 

	if (player->hasPartner() == true) {
		cout << "You found a partner!" << endl;
	} else { 
		cout << "You were unable to find a new partner :(" << endl;
	}
}

void calculateWage(Player* player) {
	player->setPartner(false);
	if (player->getJoblevel() == "Low") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((25000/24) * .78);
		} else {
			player->setWage(25000/24);
		}
	} else if (player->getJoblevel() == "Medium") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((40000/24) * .78);
		} else {
			player->setWage(40000/24);
		}
	} else if (player->getJoblevel() == "High") {
		if (player->getGenderName() == "Female" || player->getGenderName() == "Other"){
			player->setWage((100000/24) * .78);
		} else {
			player->setWage(100000/24);
		}
	}

	if (player->gethouse() == "Small house") {
		player->setLivingCost(1000);
	} else if (player->gethouse() == "Medium house") {
		player->setLivingCost(1250);
	} else if (player->gethouse() == "Large house!") {
		player->setLivingCost(1500);
	}

}

void print_information(Player* player) {
	cout << endl << "------------Players stats-----------" << endl;
	cout << "Food level: " << player->getFoodLevel() << endl;
	cout << "Wage level: " + player->getJoblevel() << endl;
	cout << "Savings: " << player->getSavings() << endl;
	cout << "House Size: " + player->gethouse() << endl;
	if (player->hasPartner() == true) {
		cout << "Relationship status: In a relationship" << endl;
	} else {
		cout << "Relationship status: Single" << endl;
	}
	cout << "------------------------------------" << endl << endl;
}

int main () {
	Player* player = new Player();
	player->init();
	cout << "Welcome to the Canadian Life Simulator!" << endl; 
	Sleep(2000); // 1 sec == 1000 ms, so sleep for 2 seconds
	cout << "You will be spawned into the world shortly!" << endl;
	Sleep(3000);
	cout << "Just like the real world, you will be assigned completely random characteristics!" << endl;
	Sleep(3000);
	cout << "Lets begin by first determining your ethnicity." << endl;
	Sleep(5000);
	srand (time(NULL));

	/* Generation of the players race will be done now */
	string race[5] = {"White", "Asian", "Black", "Indigenous", "Indian (east)" };

	int rnd = rand() % 5; // Generates random number between 0 and 4

	player->setRace(race[rnd]);

	cout << "You were born of " + race[rnd] + " ethnicity!" << endl << endl;
	Sleep(4000);
	/* this value will be between 1 and 3. 1 - male, 2 - female, 3 - other */
	cout << "Next, we will determine your gender." << endl;
	Sleep(4000);
	int gender = 0; 
	string sextype[3] = {"Heterosexual", "Homosexual", "Other"};

	/* Generates a random number between 0 and 2 for races */
	gender = rand() % 10 + 1;

	if (gender <= 4 ) { // This will be for a male 
		player->setGender(0, "Male");
	} else if (gender <= 8) { // This will be female
		player->setGender(1, "Female");
	} else if (gender >= 9) {
		player->setGender(2, "Other");
	}

	cout << "Your gender is " + player->getGenderName() << "." << endl << endl;
	Sleep(4000);
	/* this section will generate the players sexual orientation */

	cout << "Finally, we will determine your sexual orientation." << endl;
	Sleep(4000);
	rnd = rand() % 3; // Generate a random number between 0 and 2.

	player->setSex(rnd, sextype[rnd]);

	cout << "Your sexual orientation is: " + player->getSexuaL() << "." << endl << endl;
	Sleep(4000);
	bool game = true;
	
	cout << endl;
	cout << "Now you will begin to live your life." << endl;
	Sleep(4000);
	cout << "Each turn you can pursue a different aspect of life." << endl;
	Sleep(4000);
	cout << "Your characters traits affect your chances to achieve different things." << endl << endl;
	Sleep(4000);
	cout << "The objective of the game is to get a large house, a high paying job, and be in a relationship." << endl << endl;
	Sleep(5000);
	cout << "Next, we need to get you somewhere to live." << endl;
	Sleep(4000);
	cout << "Your race effects the type of house you are able to get." << endl;
	Sleep(4000);
	cout << "Buying your new house now!" << endl << endl;
	Sleep(1000);
	player = generatehouse(player); 
	cout << "You were able to purchase a " + player->gethouse() << " congratulations!" << endl << endl;
	Sleep(4000);
	int billing = 5; // counts down from 5 each turn, at 1 player is billed their billing amount
	

	cout << "Now we need to get you a job!" << endl;
	Sleep(3000);
	player = startjob(player);
	cout << "You started work at a " + player->getJoblevel() + " paying job." << endl;
	cout << endl;

	/* more intro stuff here */
	cout << "Every two turns you be paid a wage, which you can use to purchase various things!" << endl;
	Sleep(4000);
	cout << "If you run out of money you will lose the game." << endl << endl;
	Sleep(4000);
	cout << "Your character also has a food level that starts at 10." << endl;
	Sleep(4000);
	cout << "Each turn, your food level will drop by 2." << endl;
	Sleep(4000);
	cout << "If your food level hits zero, you will lose the game." << endl;
	Sleep(4000);
	cout << "You can replenish your food level by purchasing groceries for $500." << endl << endl;
	Sleep(4000);
	cout << "Lets get started!" << endl << endl;
	Sleep(3000);
	/* Beginning of main game loop now that the player has had their stats initalized. */
	// at the end of each turn the player will "go to work" so then they will get paid. 
	int actiondone = 1; // if this goes to zero the player will have no more moves.
	int turncounter = 1;
	int billcounter = 1;
	bool player_warned_job = false;
	bool player_warned_house = false;
	string input; 

	while (game) {
		if (billcounter == 5) {
			cout << "Paying bills..." << endl;
			Sleep(4000);
			player->payBills();
			billcounter = 1;
		}
		// This is the beginning of a players turn.
		if (player->getFoodLevel() < 0) {
			cout << "You have run out of food!..." << endl;
			Sleep(3000);
			cout << "Game over!" << endl << endl;
			Sleep(20000);
			return 0;
		}

		if (player->getSavings() < 0) {
			cout << "You have gone bankrupt!...." << endl;
			Sleep(3000);
			cout << "Game over!" << endl << endl;
			Sleep(20000);
			return 0;
		}

		if (player->hasPartner() == true && player->getJoblevel() == "High" && player->gethouse() == "Large house!") {
			cout << "Congradulations, you have won the game!" << endl;
			Sleep(20000);
			return 0;
		}

		if (turncounter == 2) {
			cout << "Payday!" << endl;
			player->addMoney();
			turncounter = 0;
		}

		if (player->hasPartner()) {
			rnd = rand() % 100 +1; // Generate a random number between 1-10 

			if (player->getSexuaL() == "Heterosexual") {
				if (rnd <= 30) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			} else if (player->getSexuaL() == "Homosexual") {
				if (rnd <= 35) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			} else if (player->getSexuaL() == "Other") {
				if (rnd <= 40) {
					cout << endl << "Oh no! Your partner has left you!" << endl;
					calculateWage(player);
				}
			}

		}

		actiondone = 1;

		while (actiondone == 1) {
			// at the start of each turn print out the players information. 
			// Displays on screen all the characters stats
			cout << "Bills due in : " << (5-billcounter) << " turns." << endl;
			cout << "Payday in: " << (2-turncounter) << " turns." << endl;
			print_information(player);

			cout << "What would you like do?" << endl;
			cout << "1. Look for a relationship." << endl;
			cout << "2. Search for a new job." << endl;
			cout << "3. Look for a new house ($1000)." << endl;
			cout << "4. Buy groceries ($500)." << endl;
			cout << "5. display character info." << endl;
			cout << "6. Pass (use this if you don't want to do anything this turn)." << endl << endl;
			cout << "Enter a number between 1 and 6 then press enter (q to quit)." << endl;	
		
			getline (cin,input);

			while(input != "1" && input != "2" && input != "3" && input != "4" && input != "5" && input != "q" && input != "Q" && input != "6" && input != "showmethecowlevel") {
				cout << "Invalid input, please enter a number between 1 and 6." << endl;
				getline (cin,input);
			}

			if (input == "1") {
				FindPartner(player);
				actiondone--;
			} else if (input == "2") {
				if (player_warned_job == false) {
					cout << "Warning! Searching for a new job can result in you getting a worse job than before." << endl;
					cout << "Do you wish to continue? (enter y or n)" << endl;
					player_warned_job = true;
					getline (cin, input);
					while (input != "n" && input != "y") {
						cout << "invalid input, please enter y or n" << endl;
						getline(cin,input);
					}
					/* check players input, continue or reset if y or n. */
					if (input == "y") {
						player = startjob(player);
						cout << "You got a " + player->getJoblevel() + " paying job." << endl;
						Sleep(2000);
						actiondone--;
					} else {
						continue;
					}
				} else {
					player = startjob(player);
					cout << "You got a " + player->getJoblevel() + " paying job." << endl;
					Sleep(2000);
					actiondone--;
				}
			} else if (input == "3") {
				if (player_warned_house == false) {
					cout << "Warning! Searching for a new house can result in you getting a worse house than what you had before." << endl;
					cout << "Do you wish to continue? (enter y or n)." << endl;
					player_warned_house = true;
					getline (cin, input);
					while (input != "n" && input != "y") {
						cout << "invalid input, please enter y or n." << endl;
						getline(cin,input);
					}
					/* check players input, continue or reset if y or n. */
					if (input == "y") {
						if (player->getSavings() < 1000) {
							cout << "You cannot afford to buy a new house. It costs $1000." << endl;
							continue;
						} else {
							player->subtractMoney(1000);
							player = generatehouse(player);
							cout << "You were able to purchase a " + player->gethouse() << endl << endl;
							Sleep(2000);
							actiondone--;
						}
					} else {
						continue;
					}
				} else {
					if (player->getSavings() < 1000) {
						cout << "You cannot afford to buy a new house. It costs $1000." << endl;
						continue;
					} else {
						player->subtractMoney(1000);
						player = generatehouse(player);
						cout << "You were able to purchase a " + player->gethouse() << "." << endl << endl;
						Sleep(2000);
						actiondone--;
					}
				}
			} else if (input == "4") {
				if(player->getSavings() <= 500) {
					cout << "You cannot afford groceries. They cost $500." << endl;
				} else {
					cout << "Buying groceries..." << endl << endl;
					Sleep(3000);
					player->subtractGroceries();
					player->addGroceries();
					actiondone--;
				}
			} else if (input == "5") {
				print_information(player);
			} else if (input == "q" || input == "Q") {
				return 0;
			} else if (input == "6") {
				actiondone--;
			} else if (input == "showmethecowlevel") {
				player->setWage((100000/24) * 1.75);
				player->setJobStatus(true); // player now has a job
				player->setJobLevel(3);
				player->setHouse(3); // Player owns b
				player->setHasHouse(true);
				player->addCosts(1000);
				player->setPartner(true);
				actiondone--;
			}
		}

		cout << "End of turn, going to work..." << endl << endl;
		Sleep(3000);
		player->removeFood(); 
		turncounter++;
		billcounter++;
	}
	return 0; 
>>>>>>> 79e6ba8cb99c109657b3702c56035b968dfda12e
}