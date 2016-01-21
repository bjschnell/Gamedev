import pygame, sys, os
from pygame.locals import *
from constants import *
from game_board import *
from itertools import chain

class Player:
    def __init__(self, board, player_id):
        self.board = board
        self.characters = []
        self.actions = TURN_ACTIONS
        self.ID = player_id

    def add_character(self, character):
        self.characters.append(character)
        self.board.grid[character.x, character.y] = character

    def remove_character(self, character):
        self.characters.remove(character)
        self.board.grid[character.x, character.y] = GridTile(character.x, character.y)
    def clear_characters(self):
        self.characters = []

    def move_character(self, current_tile, x, y):
        if current_tile in self.characters:
        	if x==current_tile.x or y==current_tile.y:
        	    if x in range(current_tile.x-current_tile.move_ability_distance, current_tile.x+current_tile.move_ability_distance+1):
        	        if y in range(current_tile.y-current_tile.move_ability_distance, current_tile.y+current_tile.move_ability_distance+1):
        	            temp_tile = self.board.grid[(x,y)]
        	            temp_coordinates = (current_tile.x, current_tile.y)
                        self.board.grid[(x,y)] = current_tile
                        self.board.grid[(x,y)].set_coordinates(x, y)
                        self.board.grid[(temp_coordinates[0], temp_coordinates[1])] = temp_tile
                        self.board.grid[(temp_coordinates[0], temp_coordinates[1])].set_coordinates(temp_coordinates[0], temp_coordinates[1])
                        self.use_action()

    def attack_character(self, current_tile, x, y):
        if current_tile in self.characters:
            if x in range(current_tile.x-current_tile.attack_ability_distance, current_tile.x+current_tile.attack_ability_distance+1):
                if y in range(current_tile.y-current_tile.attack_ability_distance, current_tile.y+current_tile.attack_ability_distance+1):
                    attacked_tile = self.board.grid[(x,y)]
                    attacked_tile.health_points = attacked_tile.health_points - current_tile.attack_power
                    self.use_action()

    def use_action(self):
        self.actions = self.actions - 1

    def restore_actions(self):
        self.actions = TURN_ACTIONS

    def get_actions(self):
        return self.actions
        
    def get_player(self):
        return self.ID

    def has_actions(self):
        if self.actions > 0:
            return True
        else:
            return False


class Game:

    def __init__(self):
        self.windowSurface = pygame.display.set_mode((WINDOW_HEIGHT, WINDOW_WIDTH), 0, 32)
        self.grid = Grid()
        self.player1 = Player(self.grid, "Player 1")
        self.player2 = Player(self.grid, "Player 2")
        self.player_turn = 0

    @staticmethod
    def text_objects(text, font):
        textSurface = font.render(text, True, (BLACK))
        return textSurface, textSurface.get_rect()

    @staticmethod
    def text_objects_white(text, font):
        textSurface = font.render(text, True, (WHITE))
        return textSurface, textSurface.get_rect()
    
    def display_turn_status(self, text):
        largeText = pygame.font.Font('freesansbold.ttf',60)
        TextSurf, TextRect = self.text_objects(text, largeText)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(100))
        self.windowSurface.blit(TextSurf, TextRect)

    def display_instructions(self):
        text = "Move character - m"
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(500))
        self.windowSurface.blit(TextSurf, TextRect)

        text = "Attack - a"
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(514))
        self.windowSurface.blit(TextSurf, TextRect)

        text = "New Game - n"
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(528))
        self.windowSurface.blit(TextSurf, TextRect)

    def clear_player_moves(self, playermoves):
        #this chunk acts as an eraser to get rid of the previous text first
        text = "Remaining moves: " + str(playermoves)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects_white(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(150))
        self.windowSurface.blit(TextSurf, TextRect)

    # displays the Stats of the currently selected character. 
    def display_moves(self, playermoves):
        # This chunk sets the text to the current moves the player has
        text = "Remaining moves: " + str(playermoves)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(150))
        self.windowSurface.blit(TextSurf, TextRect)

    def display_info(self, current_tile):
        text = "Health: " + str(current_tile.health_points)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextErase, TextRect_E = self.text_objects_white(text,small_text)
        TextRect_E.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(250))
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(250))
        self.windowSurface.blit(TextErase, TextRect_E)
        self.windowSurface.blit(TextSurf, TextRect)

        text = "Move distance: " + str(current_tile.move_ability_distance)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextErase, TextRect_E = self.text_objects_white(text,small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(300))
        self.windowSurface.blit(TextSurf, TextRect)

        text = "Attack Power " + str(current_tile.attack_power)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(350))
        self.windowSurface.blit(TextSurf, TextRect)

        text = "Range: " + str(current_tile.attack_ability_distance)
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(400))
        self.windowSurface.blit(TextSurf, TextRect)

    def new_game_setup(self):
        pygame.init()
        pygame.display.set_caption("Square Master Blasters")

        basic_font = pygame.font.SysFont(None, 48)
        self.windowSurface.fill(WHITE)
        self.display_instructions()
        self.display_turn_status("Player 1")
        self.player_turn = 0
        pygame.display.update()
        self.player1.clear_characters()
        self.player2.clear_characters()
        self.grid.clear_grid()

        # for testing our pilot project, I have placed two random characters for each player
        # I've modified these to now declare which player each belongs to.

        # Player one characters initalization.
        self.player1.add_character(Wilfred(4,9, PLAYER_ONE))
        self.player1.add_character(Doge(1,3, PLAYER_ONE))
        self.player1.add_character(Barney(2,5, PLAYER_ONE))
        self.player1.add_character(Bridget(4,7, PLAYER_ONE))
       	self.player1.add_character(Justin(3,11, PLAYER_ONE))
        # Player two characters initalization here
        self.player2.add_character(Moad(12,9, PLAYER_TWO))
        self.player2.add_character(George(7,3, PLAYER_TWO))
        self.player2.add_character(Shia(12,5, PLAYER_TWO))
        self.player2.add_character(James(8,7, PLAYER_TWO))
        self.player2.add_character(Trumph(7,11, PLAYER_TWO))
        
    def run(self):
        self.new_game_setup()
        # game loop
        current_player = self.player1
        other_player = self.player2
        current_tile = None
        move = False
        attack = False
        self.player_turn = 1

        while True:
            while current_player.has_actions():
                if current_tile in current_player.characters or current_tile in other_player.characters:
                    self.windowSurface.fill(WHITE, (WINDOW_WIDTH+10,CONTROL_PANEL_WIDTH/2,262,290))
                    self.display_info(current_tile)
                else:
                	self.windowSurface.fill(WHITE, (WINDOW_WIDTH+10, CONTROL_PANEL_WIDTH/2,262,290))
                pygame.display.update()
                for event in pygame.event.get():
                    if move is True:
                        if event.type == MOUSEBUTTONUP:
                            if event.pos[0] < SQUARE_SIZE*HOR_SQUARES:
                                xpos = event.pos[0]//SQUARE_SIZE
                                ypos = event.pos[1]//SQUARE_SIZE
                                move_tile = self.grid.grid[(xpos,ypos)]
                                if current_tile in current_player.characters and move_tile not in other_player.characters:
                                    current_player.move_character(current_tile, xpos,ypos)
                                    move = False
                                move = False

                    elif attack is True:
                        if event.type == MOUSEBUTTONUP:
                            if event.pos[0] < SQUARE_SIZE*HOR_SQUARES:
                                xpos = event.pos[0]//SQUARE_SIZE
                                ypos = event.pos[1]//SQUARE_SIZE
                                attack_tile = self.grid.grid[(xpos, ypos)]
                                if current_tile in current_player.characters and attack_tile in other_player.characters:
                                    current_player.attack_character(current_tile, xpos, ypos)
                                    attack = False
                                    # remove character if dead
                                    if attack_tile.health_points <= 0:
                                        other_player.remove_character(attack_tile)

                                attack = False
                    else:
                        if event.type == QUIT:
                            pygame.quit()
                            sys.exit()
                        elif event.type == MOUSEBUTTONUP:
                            if event.pos[0] < SQUARE_SIZE*HOR_SQUARES:
                                xpos = event.pos[0]//SQUARE_SIZE
                                ypos = event.pos[1]//SQUARE_SIZE
                                current_tile = self.grid.select_tile((xpos, ypos))
                        elif event.type == KEYDOWN:
                            if event.key == K_m:
                                move = True
                            if event.key == K_a:
                                attack = True
                            if event.key == K_n:
                                self.run()
                self.grid.draw(self.windowSurface, self.player_turn)
                # This will display how many moves the current player has.
                self.clear_player_moves(current_player.get_actions()+1) 
                self.display_moves(current_player.get_actions())

            # change turns
            if current_player is self.player1:
                current_player = self.player2
                other_player = self.player1
                self.player_turn = 2
                self.player1.restore_actions()
                self.windowSurface.fill(WHITE)
                self.display_turn_status(current_player.get_player())
                self.display_instructions()
                self.clear_player_moves(current_player.get_actions()+1) 
                self.display_moves(current_player.get_actions())
            else:
                current_player = self.player1
                other_player = self.player2
                self.player_turn = 1
                self.player2.restore_actions()
                self.windowSurface.fill(WHITE)
                self.display_turn_status(current_player.get_player())
                self.display_instructions()
                self.clear_player_moves(current_player.get_actions()+1) 
                self.display_moves(current_player.get_actions())

            if len(current_player.characters) == 0:
                self.windowSurface.fill(WHITE)
                if current_player == self.player1:
                    self.display_turn_status("Player 2 Won!")
                else:
                    self.display_turn_status("Player 1 Won!")


if __name__ == '__main__':
    Game().run()
