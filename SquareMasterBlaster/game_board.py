import constants
import pygame, os
from constants import *

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
        self.move_ability_distance = None
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = None
        self.attack_power = None
        # here you specify your characters health points
        self.health_points = None

    def draw(self, windowSurface, player_turn):
        X = self.x*SQUARE_SIZE+GRID_LINE_THICKNESS
        Y = self.y*SQUARE_SIZE+GRID_LINE_THICKNESS
        self.draw_border(windowSurface)
        if self.selected:
            # changes color of character tile selection depending on ownership
            if player_turn is self.owner:
                # this line colors the outside of the square Yellow if it is the players.
                #self.draw_display()
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

    def clear_grid(self):
        self.grid = {(x, y): GridTile(x,y) for x in range(HOR_SQUARES) for y in range(VERT_SQUARES)}
        
#-------------------------------------
# This class is a template to create extra characters from
# copy the class and replace/set the values accordingly
# ------------------------------------
class Characternamehere(Character):
    def __init__(self, x,y,owner):
        super(George, self).__init__(x,y,owner)
        # These two load the image specified
        self.image = pygame.image.load(os.path.join('img', 'example.jpeg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move Distance
        self.move_ability = 1 # Scales between 1-10
        # Attack Distance
        self.attack_ability_distance = 1 # Scales between 1-10
        # Attack value
        self.attack_power = 1 # Scales between 1-10
        # Health points
        self.health_points = 1 # Scales between 1-10

class Wilfred(Character):
    def __init__(self, x,y,owner):
        super(Wilfred, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'WilfredFINAL.jpg')).convert()
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
        self.image = pygame.image.load(os.path.join('img', 'JamesFINAL.jpg')).convert()
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
        self.attack_ability_distance = 2
        self.attack_power = 3
        # HEALTH POINTS
        self.health_points = 4

class Moad(Character):
    def __init__(self, x,y,owner):
        super(Moad, self).__init__(x,y,owner)
        # replace with image file you want for you character (put the file in the /img directory)
        self.image = pygame.image.load(os.path.join('img', 'MoadFINAL.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # here you specify how far your character can move in a single action
        self.move_ability_distance = 1
        # here you specify how many hp your character takes down in a single action
        self.attack_ability_distance = 3
        self.attack_power = 2
        # here you specify your characters health points
        self.health_points = 10

class Barney(Character):
    def __init__(self, x,y,owner):
        super(Barney, self).__init__(x,y,owner)
        # Load barney into the game yo.
        self.image = pygame.image.load(os.path.join('img', 'barney.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move distance
        self.move_ability_distance = 3
        # Attack power
        self.attack_ability_distance = 3
        self.attack_power = 3

        # Health Points
        self.health_points = 5

# George is meant to be semi-overpowered.
class George(Character):
    def __init__(self, x,y,owner):
        super(George, self).__init__(x,y,owner)
        # George has entered the game.
        self.image = pygame.image.load(os.path.join('img', 'georgeFINAL.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move Distance
        self.move_ability_distance = 1
        # Attack power and distance
        self.attack_ability_distance = 1
        self.attack_power = 10

        # Health points
        self.health_points = 10

class Shia(Character):
    def __init__(self, x,y,owner):
        super(Shia, self).__init__(x,y,owner)
        # These two load the image specified
        self.image = pygame.image.load(os.path.join('img', 'shia.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move Distance
        self.move_ability_distance = 5 # Scales between 1-10
        # Attack Distance
        self.attack_ability_distance = 3 # Scales between 1-10
        # Attack value
        self.attack_power = 2 # Scales between 1-10
        # Health points
        self.health_points = 6 # Scales between 1-10

class Justin(Character):
    def __init__(self, x,y,owner):
        super(Justin, self).__init__(x,y,owner)
        # These two load the image specified
        self.image = pygame.image.load(os.path.join('img', 'trudeau.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move Distance
        self.move_ability_distance = 7 # Scales between 1-10
        # Attack Distance
        self.attack_ability_distance = 2# Scales between 1-10
        # Attack value
        self.attack_power = 5 # Scales between 1-10
        # Health points
        self.health_points = 6 # Scales between 1-10


class Trumph(Character):
    def __init__(self, x,y,owner):
        super(Trumph, self).__init__(x,y,owner)
        # These two load the image specified
        self.image = pygame.image.load(os.path.join('img', 'trumph.jpg')).convert()
        self.image = pygame.transform.scale(self.image,(GRID_IMAGE_DIMENSIONS,GRID_IMAGE_DIMENSIONS))
        # Move Distance
        self.move_ability_distance = 2 # Scales between 1-10
        # Attack Distance
        self.attack_ability_distance = 2 # Scales between 1-10
        # Attack value
        self.attack_power = 5 # Scales between 1-10
        # Health points
        self.health_points = 4 # Scales between 1-10