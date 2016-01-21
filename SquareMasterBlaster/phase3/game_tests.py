if __name__ == '__main__' and __package__ is None:
    from os import sys, path
    sys.path.append(path.dirname(path.dirname(path.abspath(__file__))))


import unittest
import pygame
from game import Game, Grid, Player, Character, GridTile

class TestPlayerMethods(unittest.TestCase):
    def setUp(self):
        self.game = Game()
        self.grid = Grid()
        self.player = Player(self.grid)

    def test_add_character(self):
        character = Character(5,6,self.player)
        self.player.add_character(character)
        self.assertEquals(character, self.grid.grid[(5,6)])
        self.assertIn(character, self.player.characters)
        self.assertEquals(1, len(self.player.characters))

    def test_remove_character(self):
        character = Character(5,6,self.player)
        self.player.add_character(character)
        self.player.remove_character(character)
        self.assertIsInstance(self.grid.grid[(5,6)], GridTile)
        self.assertEquals(0, len(self.player.characters))

    def test_move_character(self):
        character = Character(5,6,self.player)
        self.player.add_character(character)
        self.player.move_character(character, 5, 7)
        self.assertEquals(character, self.grid.grid[5, 7])

    def test_attack_character(self):
        character = Character(5,6,self.player)
        character2 = Character(5,7,self.player)
        initial_health_points = character.health_points
        initial_attack_points = character2.attack_power
        self.player.add_character(character)
        self.player.add_character(character2)
        self.player.attack_character(character2, 5, 6)
        self.assertEquals(character.health_points, initial_health_points-initial_attack_points)


if __name__ == '__main__':
    unittest.main()