/*****************************************************
	Canadian Life simulator 
	Created By Brady Schnell
	Project start date Dec 04 2015
	Last update: Dec 04 2015
	
	This game is designed to demonstrate the level of discrimination that
	canadians of different upbringings face on a daily level. 
******************************************************/
#include <cstdlib>
#include <string>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
//#include <windows.h> to be included later to make game flow better

#define MAX_SALARY 100,000

using namespace std;

class Player {
	private:
		///<---- initalization variables for players character
		string race; // Tracks the players Race
		int gender; // Players Gender
		string gender_name; // Players gender in string
		int sex_num; // int representing type of sexual orientation
		string sex_type; // string for sexual orientation

		///<---- players game stats.
		float wage; // Players wages 
		float savings;  // Savings player currently has
		float discrim; // this will be a value between 0 and 1
		bool has_partner; //  represents whether the player has a parner or not.
		int food; // 10 is max food. if it goes negative player loses.
		int house_size; // will be a value from 1-3 from smallest to largest
	public:
		void init() { 
			has_partner = false;
			savings = 0.0f;
			wage = 0.0f;
			discrim = 0.0f;
			food = 10;
			house_size = 0; 
		}

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
};

int main () {
	Player* player = new Player();
	player->init();
	cout << "Welcome to the Canadian life simulator!" << endl; 
//	sleep(2000); // 1 sec == 1000 ms, so sleep for 2 seconds
	cout << "You will be spawned into the world shortly!" << endl;
	cout << "Just like the real world, you will be assigned completely random characteristics!" << endl;
	cout << "Lets begin by first determining your ethnicity." << endl;

	srand (time(NULL));

	/* Generation of the players race will be done now */
	string race[5] = {"White", "Asian", "Black", "Indigenous", "Indian (east)" };

	int rnd = rand() % 5; // Generates random number between 0 and 2

	player->setRace(race[rnd]);

	cout << "You were born of " + race[rnd] + " ethnicity!" << endl;

	/* this value will be between 1 and 3. 1 - male, 2 - female, 3 - other */
	cout << "Next, we will determine your gender." << endl;
	int gender = 0; 
	string sextype[3] = {"Heterosexual", "Homosexual", "Other"};

	/* Generates a random number between 0 and 2 for races */
	gender = rand() % 3;

	if (gender == 0 ) { // This will be for a male 
		player->setGender(gender, "Male");
	} else if (gender == 1) { // This will be female
		player->setGender(gender, "Female");
	} else if (gender == 2) {
		player->setGender(gender, "Other");
	}

	cout << "Your gender is " + player->getGenderName() << endl;

	/* this section will generate the players sexual orientation */

	cout << "Finally, we will determine your sexual orientation." << endl;

	rnd = rand() % 3; // Generate a random number between 0 and 2.

	player->setSex(rnd, sextype[rnd]);

	cout << "Your sexual orientation is: " + player->getSexuaL() << endl;

	bool game = true;

	cout << endl;
	cout << "Now you will begin to live your life." << endl;
	cout << "Each turn you can pursue a different life aspect" << endl;
	cout << "Your characters traits effect your chances to achieve different things." << endl;

	/* Beginning of main game loop now that the player has had their stats initalized. */
	while (game) {





	}
	return 0; 
}