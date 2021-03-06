# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: buffer.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='buffer.proto',
  package='seng330',
  serialized_pb=_b('\n\x0c\x62uffer.proto\x12\x07seng330\"\xe0\x01\n\x06Player\x12-\n\ncharacters\x18\x01 \x03(\x0b\x32\x19.seng330.Player.Character\x1a\x62\n\tCharacter\x12\t\n\x01x\x18\x01 \x02(\x05\x12\t\n\x01y\x18\x02 \x02(\x05\x12&\n\x04name\x18\x03 \x02(\x0e\x32\x18.seng330.Player.charName\x12\x17\n\x0fremaininghealth\x18\x04 \x02(\x05\"C\n\x08\x63harName\x12\x0b\n\x07Wilfred\x10\x01\x12\t\n\x05James\x10\x02\x12\x0b\n\x07\x42ridget\x10\x03\x12\x08\n\x04\x44oge\x10\x04\x12\x08\n\x04Moad\x10\x05\"V\n\x04Game\x12\'\n\x0e\x63urrent_player\x18\x01 \x02(\x0b\x32\x0f.seng330.Player\x12%\n\x0cother_player\x18\x02 \x02(\x0b\x32\x0f.seng330.Player')
)
_sym_db.RegisterFileDescriptor(DESCRIPTOR)



_PLAYER_CHARNAME = _descriptor.EnumDescriptor(
  name='charName',
  full_name='seng330.Player.charName',
  filename=None,
  file=DESCRIPTOR,
  values=[
    _descriptor.EnumValueDescriptor(
      name='Wilfred', index=0, number=1,
      options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='James', index=1, number=2,
      options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Bridget', index=2, number=3,
      options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Doge', index=3, number=4,
      options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Moad', index=4, number=5,
      options=None,
      type=None),
  ],
  containing_type=None,
  options=None,
  serialized_start=183,
  serialized_end=250,
)
_sym_db.RegisterEnumDescriptor(_PLAYER_CHARNAME)


_PLAYER_CHARACTER = _descriptor.Descriptor(
  name='Character',
  full_name='seng330.Player.Character',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='x', full_name='seng330.Player.Character.x', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='y', full_name='seng330.Player.Character.y', index=1,
      number=2, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='name', full_name='seng330.Player.Character.name', index=2,
      number=3, type=14, cpp_type=8, label=2,
      has_default_value=False, default_value=1,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='remaininghealth', full_name='seng330.Player.Character.remaininghealth', index=3,
      number=4, type=5, cpp_type=1, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=83,
  serialized_end=181,
)

_PLAYER = _descriptor.Descriptor(
  name='Player',
  full_name='seng330.Player',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='characters', full_name='seng330.Player.characters', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
  ],
  extensions=[
  ],
  nested_types=[_PLAYER_CHARACTER, ],
  enum_types=[
    _PLAYER_CHARNAME,
  ],
  options=None,
  is_extendable=False,
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=26,
  serialized_end=250,
)


_GAME = _descriptor.Descriptor(
  name='Game',
  full_name='seng330.Game',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='current_player', full_name='seng330.Game.current_player', index=0,
      number=1, type=11, cpp_type=10, label=2,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='other_player', full_name='seng330.Game.other_player', index=1,
      number=2, type=11, cpp_type=10, label=2,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=252,
  serialized_end=338,
)

_PLAYER_CHARACTER.fields_by_name['name'].enum_type = _PLAYER_CHARNAME
_PLAYER_CHARACTER.containing_type = _PLAYER
_PLAYER.fields_by_name['characters'].message_type = _PLAYER_CHARACTER
_PLAYER_CHARNAME.containing_type = _PLAYER
_GAME.fields_by_name['current_player'].message_type = _PLAYER
_GAME.fields_by_name['other_player'].message_type = _PLAYER
DESCRIPTOR.message_types_by_name['Player'] = _PLAYER
DESCRIPTOR.message_types_by_name['Game'] = _GAME

Player = _reflection.GeneratedProtocolMessageType('Player', (_message.Message,), dict(

  Character = _reflection.GeneratedProtocolMessageType('Character', (_message.Message,), dict(
    DESCRIPTOR = _PLAYER_CHARACTER,
    __module__ = 'buffer_pb2'
    # @@protoc_insertion_point(class_scope:seng330.Player.Character)
    ))
  ,
  DESCRIPTOR = _PLAYER,
  __module__ = 'buffer_pb2'
  # @@protoc_insertion_point(class_scope:seng330.Player)
  ))
_sym_db.RegisterMessage(Player)
_sym_db.RegisterMessage(Player.Character)

Game = _reflection.GeneratedProtocolMessageType('Game', (_message.Message,), dict(
  DESCRIPTOR = _GAME,
  __module__ = 'buffer_pb2'
  # @@protoc_insertion_point(class_scope:seng330.Game)
  ))
_sym_db.RegisterMessage(Game)


# @@protoc_insertion_point(module_scope)
