import pygame, sys, os
from pygame.locals import *
from itertools import chain

# pygame constants
SQUARE_SIZE = 50
CONTROL_PANEL_WIDTH = 400
VERT_SQUARES = 16
HOR_SQUARES = 16
WINDOW_WIDTH = SQUARE_SIZE*VERT_SQUARES
WINDOW_HEIGHT = SQUARE_SIZE*HOR_SQUARES + CONTROL_PANEL_WIDTH
PLAYER_ONE = 1
PLAYER_TWO = 2

#Colour declarations! Channels = GREED BLUE RED - 
BLACK = (0,0,0)
WHITE = (255, 255, 255)
GREY = (127,127,127)
GREEN_TRANSPARENT = (0, 255, 0, 128)
RED_TRANSPARENT = (0, 51, 51, 255)
BLUE_TRANSPARENT = (0, 128, 255, 0)
YELLOW_TRANSPARENT = (0, 255, 255, 0)

GRID_LINE_THICKNESS = 2
GRID_IMAGE_DIMENSIONS = SQUARE_SIZE - GRID_LINE_THICKNESS*2

# Gameplay constants
TURN_ACTIONS = 2

class GridTile(object):

    def __init__(self,x,y):
        self.x = x
        self.y = y
        self.selected = False

    def select(self):
        self.selected = True

    def deselect(self):
        self.selected = False

    def set_coordinates(self, x, y):
        self.x = x
        self.y = y

    def draw(self, windowSurface , player_turn):
        X = self.x*SQUARE_SIZE+GRID_LINE_THICKNESS
        Y = self.y*SQUARE_SIZE+GRID_LINE_THICKNESS
        self.draw_border(windowSurface)
        if self.selected:
            # changes color of tile selection based upon which players turn is current
            if player_turn is 1:
                pygame.draw.rect(windowSurface, GREEN_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
            else :
                pygame.draw.rect(windowSurface, BLUE_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
        else:
            pygame.draw.rect(windowSurface, WHITE, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))

    def draw_border(self, windowSurface):
        pygame.draw.lines(windowSurface, GREY, True, [(self.x*SQUARE_SIZE,self.y*SQUARE_SIZE), (self.x*SQUARE_SIZE+SQUARE_SIZE,self.y*SQUARE_SIZE)
            , (self.x*SQUARE_SIZE+SQUARE_SIZE,self.y*SQUARE_SIZE+SQUARE_SIZE), (self.x*SQUARE_SIZE,self.y*SQUARE_SIZE+SQUARE_SIZE)], GRID_LINE_THICKNESS)


class Character(GridTile):
    def __init__(self,x,y,owner):
        self.x = x
        self.y = y
        self.selected = False
        self.owner = owner
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'beaker.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 3
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 2
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 5

    def draw(self, windowSurface, player_turn):
        X = self.x*SQUARE_SIZE+GRID_LINE_THICKNESS
        Y = self.y*SQUARE_SIZE+GRID_LINE_THICKNESS
        self.draw_border(windowSurface)
        if self.selected:
            # changes color of character tile selection depending on ownership
            if player_turn is self.owner:
                pygame.draw.rect(windowSurface, YELLOW_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
            else :
                pygame.draw.rect(windowSurface, RED_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
        else:
            # changes color of idle character tiles based on ownership of characters
            if 1 is self.owner:
                pygame.draw.rect(windowSurface, GREEN_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
            elif 1 is not self.owner: 
                pygame.draw.rect(windowSurface, BLUE_TRANSPARENT, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
            else:
                pygame.draw.rect(windowSurface, WHITE, (X,Y,SQUARE_SIZE-1,SQUARE_SIZE-1))
        windowSurface.blit(self.image,(X+1,Y+1))

    def draw_border(self, windowSurface):
        pygame.draw.lines(windowSurface, GREY, True, [(self.x*SQUARE_SIZE,self.y*SQUARE_SIZE), (self.x*SQUARE_SIZE+SQUARE_SIZE,self.y*SQUARE_SIZE)
            , (self.x*SQUARE_SIZE+SQUARE_SIZE,self.y*SQUARE_SIZE+SQUARE_SIZE), (self.x*SQUARE_SIZE,self.y*SQUARE_SIZE+SQUARE_SIZE)], GRID_LINE_THICKNESS)

class Wilfred(Character):
    def __init__(self, x,y,owner):
        super(Wilfred, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'wilfred.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 4
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 2
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 5
        
class James(Character):
    def __init__(self, x,y,owner):
        super(James, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'james.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 2
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 5
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 4
        
class Bridget(Character):
    def __init__(self, x,y,owner):
        super(Bridget, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'b.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 7
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 2
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 5

class Doge(Character):
    def __init__(self, x,y,owner):
        super(Doge, self).__init__(x,y,owner)
        # Load doge into game.
        self.image = pygame.image.load(os.path.join('img','doge.jpeg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Distance to move
        self.move_ability_distance = 1
        # ATTACK POWER
        self.attack_ability_distance = 10
        self.attack_power = 2
        # HEALTH POINTS
        self.health_points = 2
        
class Moad(Character):
    def __init__(self, x,y,owner):
        super(Moad, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'mo.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 1
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 3
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 10

class Grid:
    def __init__(self):
        self.grid = {(x, y): GridTile(x,y) for x in range(HOR_SQUARES) for y in range(VERT_SQUARES)}
        self.current_tile = None

    def draw(self, windowSurface, player_turn):
        for x in range(HOR_SQUARES):
            for y in range(VERT_SQUARES):
                self.grid[(x, y)].draw(windowSurface, player_turn)

    def select_tile(self, coordinates):
        if self.current_tile:
            self.current_tile.deselect()
        self.current_tile = self.grid[coordinates]
        self.current_tile.select()
        return self.current_tile


class Player:
    def __init__(self, board):
        self.board = board
        self.characters = []
        self.actions = TURN_ACTIONS

    def add_character(self, character):
        self.characters.append(character)
        self.board.grid[character.x, character.y] = character

    def remove_character(self, character):
        self.characters.remove(character)
        self.board.grid[character.x, character.y] = GridTile(character.x, character.y)

    def move_character(self, current_tile, x, y):
        if current_tile in self.characters:
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

    def has_actions(self):
        if self.actions > 0:
            return True
        else:
            return False


class Game:

    def __init__(self):
        self.windowSurface = pygame.display.set_mode((WINDOW_HEIGHT, WINDOW_WIDTH), 0, 32)
        self.grid = Grid()
        self.player1 = Player(self.grid)
        self.player2 = Player(self.grid)
        self.player_turn = 0

    @staticmethod
    def text_objects(text, font):
        textSurface = font.render(text, True, (BLACK))
        return textSurface, textSurface.get_rect()

    def display_turn_status(self, text):
        largeText = pygame.font.Font('freesansbold.ttf',60)
        TextSurf, TextRect = self.text_objects(text, largeText)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(100))
        self.windowSurface.blit(TextSurf, TextRect)

    def display_instructions(self):
        text = "Move character - m, Attack - a"
        small_text = pygame.font.Font('freesansbold.ttf',14)
        TextSurf, TextRect = self.text_objects(text, small_text)
        TextRect.center = ((WINDOW_WIDTH+CONTROL_PANEL_WIDTH/2),(500))
        self.windowSurface.blit(TextSurf, TextRect)

    def run(self):
        pygame.init()
        pygame.display.set_caption("SENG 330 PROJECT")

        basic_font = pygame.font.SysFont(None, 48)
        self.windowSurface.fill(WHITE)
        self.display_instructions()
        self.display_turn_status("Player 1")
        pygame.display.update()

        # for testing our pilot project, I have placed two random characters for each player
        # I've modified these to now declare which player each belongs to. 
        self.player1.add_character(Wilfred(1,1,PLAYER_ONE))
        self.player2.add_character(Character(5,5,PLAYER_TWO))
        self.player1.add_character(Doge(1,2,PLAYER_ONE))
        # game loop
        current_player = self.player1
        other_player = self.player2
        current_tile = None
        move = False
        attack = False
        self.player_turn = 1

        while True:
            while current_player.has_actions():
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

                self.grid.draw(self.windowSurface, self.player_turn)
            # change turns
            if current_player is self.player1:
                current_player = self.player2
                other_player = self.player1
                self.player_turn = 2
                self.player1.restore_actions()
                self.windowSurface.fill(WHITE)
                self.display_turn_status("Player 2")
                self.display_instructions()
            else:
                current_player = self.player1
                other_player = self.player2
                self.player_turn = 1
                self.player2.restore_actions()
                self.windowSurface.fill(WHITE)
                self.display_turn_status("Player 1")
                self.display_instructions()

if __name__ == '__main__':
    Game().run()
