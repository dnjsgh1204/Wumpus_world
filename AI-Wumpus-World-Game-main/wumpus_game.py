import random
import sys
import pygame

class Board:
    # 기본 생성자: 보드의 크기와 구덩이 확률을 초기화
    def __init__(self, size, pit_probability):
        self.size = size
        self.grid = [[None] * size for _ in range(size)]  # 세계를 나타내는 2D 그리드
        self.createWorld(pit_probability)  # 주어진 구덩이 확률로 세계 생성

        # Pygame 초기화
        pygame.init()

        # 디스플레이 설정
        self.cell_size = 100
        self.window_size = (self.size * self.cell_size, self.size * self.cell_size)
        self.screen = pygame.display.set_mode(self.window_size)
        pygame.display.set_caption('Wumpus World')

        # 이미지 로드
        self.images = {
            'tile': pygame.image.load('graphics/tile.png'),
            'gold': pygame.image.load('graphics/gold.png'),
            'wumpus': pygame.image.load('graphics/wumpus.png'),
            'pit': pygame.image.load('graphics/pit.png'),
            'player': pygame.image.load('graphics/player.png')
        }

    # 금, 웜퍼스 및 구덩이로 세계를 생성하는 함수
    def createWorld(self, pit_probability):
        def randomCell(first_row=0, first_col=0):
            row, col = (0, 0)
            while (row, col) in {(0, 0), (0, 1), (1, 0)}:
                row, col = random.randint(first_row, self.size - 1), random.randint(first_col, self.size - 1)
            return row, col

        # 금을 배치
        gold_row, gold_col = randomCell()
        self.grid[gold_row][gold_col] = ['G']

        # 웜퍼스를 배치
        wumpus_row, wumpus_col = randomCell()
        if not self.grid[wumpus_row][wumpus_col]:
            self.grid[wumpus_row][wumpus_col] = ['W']
        else:
            self.grid[wumpus_row][wumpus_col].append('W')

        # 구덩이를 배치
        for row in range(self.size):
            for col in range(self.size):
                if (row, col) not in {(0, 0), (0, 1), (1, 0)} and not self.grid[row][col] and random.random() <= pit_probability:
                    self.grid[row][col] = ['P']

        # 세계 출력
        for row in reversed(self.grid):
            print(row)

    # 보드에서 위치(x, y)가 유효한지 확인하는 함수
    def checkLocation(self, x, y):
        return 0 <= x < self.size and 0 <= y < self.size

    # 보드의 현재 상태를 표시하는 함수
    def display(self, player):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()

        # 배경 타일을 그리기
        for y in range(self.size):
            for x in range(self.size):
                tile = pygame.transform.scale(self.images['tile'], (self.cell_size, self.cell_size))
                self.screen.blit(tile, (x * self.cell_size, (self.size - y - 1) * self.cell_size))

        # 배경 타일 위에 요소 그리기
        for y in range(self.size):
            for x in range(self.size):
                cell_content = self.grid[y][x]

                if cell_content:
                    # 요소의 다른 조합을 확인하고 해당 이미지를 선택
                    if 'G' in cell_content:
                        image = pygame.transform.scale(self.images['gold'], (self.cell_size, self.cell_size))
                    elif 'W' in cell_content:
                        image = pygame.transform.scale(self.images['wumpus'], (self.cell_size, self.cell_size))
                    elif 'P' in cell_content:
                        image = pygame.transform.scale(self.images['pit'], (self.cell_size, self.cell_size))
                    else:
                        continue

                    # 현재 셀에 이미지 그리기
                    self.screen.blit(image, (x * self.cell_size, (self.size - y - 1) * self.cell_size))

        # 플레이어 그리기
        player_image = pygame.transform.scale(self.images['player'], (self.cell_size, self.cell_size))
        self.screen.blit(player_image, (player.position[1] * self.cell_size, (self.size - player.position[0] - 1) * self.cell_size))

        pygame.display.flip()

class Player:
    # 기본 생성자: 플레이어 속성 초기화
    def __init__(self):
        self.position = (0, 0)
        self.orientation = 'right'
        self.arrow = 3
        self.score = 0
        self.action = ''
        self.visited = {}  # 방문한 위치와 그 지각을 저장할 딕셔너리
        self.visited_repeat = []  # 반복 방문한 위치를 저장할 리스트
        self.has_gold = False
        self.game_over = False

    # 현재 방향을 기준으로 플레이어를 왼쪽으로 회전하는 함수
    def turnLeft(self):
        directions = ['up', 'left', 'down', 'right']
        self.orientation = directions[(directions.index(self.orientation) - 1) % 4]
        self.score -= 1

    # 현재 방향을 기준으로 플레이어를 오른쪽으로 회전하는 함수
    def turnRight(self):
        directions = ['up', 'right', 'down', 'left']
        self.orientation = directions[(directions.index(self.orientation) + 1) % 4]
        self.score -= 1

    # 현재 위치의 지각을 얻는 함수
    def getPerceptions(self, board):
        perceptions = []
        y, x = self.position

        # 인접한 셀에서 웜퍼스, 구덩이 또는 금을 확인하고 해당 지각을 추가
        for i, j in [(y+1, x), (y-1, x), (y, x+1), (y, x-1)]:
            if board.checkLocation(i, j) and board.grid[i][j] is not None:
                for p in board.grid[i][j]:
                    if p == 'W':
                        perceptions.append('Stench')
                    elif p == 'P':
                        perceptions.append('Breeze')
                    elif p == 'G':
                        perceptions.append('Glitter')
        return perceptions

    # 현재 방향으로 플레이어를 앞으로 이동하는 함수
    def moveForward(self, board):
        if self.game_over:
            return

        y, x = self.position
        if self.orientation == 'right':
            x += 1
        elif self.orientation == 'left':
            x -= 1
        elif self.orientation == 'up':
            y += 1
        elif self.orientation == 'down':
            y -= 1

        # 새로운 위치가 유효한 보드 범위 내에 있는지 확인
        if board.checkLocation(y, x):
            self.position = (y, x)
        else:
            # 에이전트가 벽에 부딪힘
            print('\tBump...')
            self.turnRight()

        y, x = self.position
        current_pos = board.grid[y][x]

        # 플레이어가 구덩이 또는 웜퍼스를 만나는지 또는 게임에서 승리하는지 확인
        if current_pos and any(element in ["P", "W"] for element in current_pos):
            print('------- GAME OVER -------')
            self.score -= 1000
            self.game_over = True
        elif current_pos and 'G' in current_pos:
            print("\t Grab Gold Found!!!")
            self.grabGold(board)

    # 금을 잡고 보드에서 제거하는 함수
    def grabGold(self, board):
        y, x = self.position
        board.grid[y][x] = None
        self.score += 1000
        self.has_gold = True

    # 장애물이나 위험 상황을 처리하는 함수
    def handleObstacles(self, board):
        if self.game_over:
            return

        if 'Breeze' in self.getPerceptions(board) or 'Stench' in self.getPerceptions(board):
            self.turnRight()
        else:
            self.moveForward(board)

    # 시작점으로 안전하게 돌아가는 함수
    def climb(self, board):
        if self.game_over:
            return

        while self.position != (0, 0):
            self.moveForward(board)
            if self.position == (0, 0):
                break
            self.turnLeft()
        print("금과 함께 시작점으로 안전하게 돌아왔습니다!")
        print("게임 종료. 승리했습니다!")
        pygame.time.delay(2000)
        pygame.quit()
        sys.exit()

    # 방문한 위치를 유지하면서 플레이어 상태를 리셋하는 함수
    def reset(self):
        self.position = (0, 0)
        self.orientation = 'right'
        self.arrow = 3
        self.action = ''
        self.has_gold = False
        self.game_over = False

class WumpusWorld:
    # 기본 생성자: 보드와 플레이어로 게임 초기화
    def __init__(self, size=4, pit_probability=0.1):
        self.board = Board(size, pit_probability)
        self.player = Player()

    # 위험한 상황(Stench 또는 Breeze)을 처리하는 함수
    def handleDanger(self, perceptions):
        if 'Stench' in perceptions:
            y, x = self.player.position
            if self.player.arrow > 0:
                # 화살이 있으면 웜퍼스를 쏘기
                self.player.action = 'Shoot'
                print("Action:", self.player.action)
                self.player.shootArrow(self.board, y, x, self.player.orientation)
            else:
                # 화살이 없으면 안전한 위치로 이동 시도
                if len(self.player.visited) > 1:
                    self.player.turnRight()
        elif 'Breeze' in perceptions:
            self.player.turnRight()

    # Wumpus World 게임을 실행하는 함수
    def play(self):
        # 프레임 속도를 제어하기 위해 Pygame 시계 초기화
        clock = pygame.time.Clock()

        # 메인 게임 루프
        while True:
            # 현재 위치를 기반으로 플레이어의 지각을 가져옴
            perceptions = self.player.getPerceptions(self.board)

            # 게임의 현재 상태를 표시
            self.board.display(self.player)

            # 방문한 타일과 지각 정보를 업데이트
            current_tile = (self.player.position[0], self.player.position[1])
            self.player.visited[current_tile] = perceptions
            self.player.visited_repeat.append(current_tile)

            # 게임의 현재 상태를 출력
            print("\n-----------------\n")
            print("\tCurrent State:")
            print("Player Position:", self.player.position)
            print("Perceptions:", perceptions)

            if 'Glitter' in perceptions:
                self.player.grabGold(self.board)
                self.player.climb(self.board)

            # 위험한 상황(Pit 또는 Wumpus)을 확인
            elif 'Breeze' in perceptions or 'Stench' in perceptions:
                self.handleDanger(perceptions)
            # 위험이 없으면 기본적으로 왼쪽으로 회전 후 이동
            else:
                self.player.handleObstacles(self.board)
                self.player.action = 'MoveForward'
                print("Action:", self.player.action)
                self.player.moveForward(self.board)

            # 플레이어의 현재 방향과 점수를 출력
            print('Orientation:', self.player.orientation)
            print("Score:", self.player.score)

            # 게임을 시각화하기 위해 지연을 추가하고 프레임 속도를 초당 30프레임으로 제한
            pygame.time.delay(1000)
            clock.tick(30)

            # 게임의 종료 조건(승리 또는 패배)을 확인
            if self.player.score <= -1000 or self.player.game_over:
                print("Restarting from (0, 0)")
                self.player.reset()

# 게임을 초기화하고 실행
world = WumpusWorld()
world.play()
