
#include "stdio.h"
#include "string.h"


#define GET_OFFSET(dir,y,x)	(dir*16+y*4+x)
#define GET_DATA(step)	(step==0?0x0620064004600260:0xf9d7f9beeb9f7d9f)
#define IS_NEED_CHECK(step,dir,y,x)	((GET_DATA(step)>>GET_OFFSET(dir,y,x))&0x1)
#define CHECK(stage,block,mode)	((block+mode*stage)==0)
#define PUT_CHECK(step,stage,block,dir,y,x,mode)	(IS_NEED_CHECK(step,dir,y,x)&&CHECK(stage,block,mode))
int count = 0;
void PrintStage(int stage[22][22])
{
	for (int y = 0; y < 22; y++)
	{
		for (int x = 0; x < 22; x++)
		{
			switch (stage[y][x])
			{
			case 1:
				printf("\x1b[31m");     /* 前景色を赤に */
				printf("■");
				break;
			case 2:
				printf("\x1b[32m");     /* 前景色を緑に */
				printf("■");
				break;
			case 4:
				printf("\x1b[33m");     /* 前景色を黄色に */
				printf("■");
				break;
			case 5:
				printf("\x1b[34m");     /* 前景色を青に */
				printf("■");
				break;
			case -1:
				printf("\x1b[31m");     /* 前景色を赤に */
				printf("□");
				break;
			case -2:
				printf("\x1b[32m");     /* 前景色を緑に */
				printf("□");
				break;
			case -4:
				printf("\x1b[33m");     /* 前景色を黄色に */
				printf("□");
				break;
			case -3:
				printf("\x1b[34m");     /* 前景色を青に */
				printf("□");
				break;
			default:
				printf("□");
				break;
			}
			
			printf("\x1b[39m");     /* 前景色をデフォルトに戻す */
		}
		printf("\n");
	}
	
}
bool PutBlock(int startY, int startX, int endY, int endX, int block[2][5], int stage[22][22])
{
	count++;
	printf("=====%d====\n");
	PrintStage(stage);

#if 1
	// 終了判定
	if ((block[0][1] || block[0][2] || block[0][3] || block[0][4])==0)
	{
		if ((block[1][1] || block[1][2] || block[1][3] || block[1][4])==0)
		{
			return true;
		}
		return false;
	}
	int stageTemp[22][22];
	memcpy(stageTemp, stage, 4 * 22 * 22);
	// 全ステージ
	int* pStage = &stage[startY][startX];
	for (int y = startY; y < endY; y++)
	{
		for (int x = startX; x < endX; x++)
		{
			// 設置済ブロックと隣接しない場合は次へ
			if (y>0&&stage[y-1][x]<=0||x>stage[y][x]<=0)
			{
				continue;
			}
			// 全ブロックタイプ
			for (int type = 1; type <= 4; type++)
			{
				// 自由に使えるブロックと事前設置されたブロック順
				for (int kind = 0; kind < 2; kind++)
				{
					// ブロック全部設置したら、次へ
					if (block[kind][type]==0)
					{
						continue;
					}
					// 隣接ブロック同色の場合、次へ
					if ((x > 0 && (stage[y][x-1] == type || (y > 0 && stage[y-1][x-1] == type))) || (y > 0 && (stage[y-1][x] == type || stage[y-1][x+1] == type)))
					{
						continue;
					}
					block[kind][type]--;
					//                 ■■   ■■   □■   ■□
					// 全ブロック角度[0:■□ 1:□■ 2:■■ 3:■■ ]
					for (int dir = 0; dir < 4; dir++)
					{
						if ((kind==0 && (stage[y][x]==0||(dir==2&& stage[y][x] > 0)) && (stage[y][x+1]==0||dir==3) && (stage[y+1][x]==0||dir==1) && (stage[y+1][x+1]==0||dir==0))
						 )// ||(kind>0 && ()))
						{
							stage[y][x] == type;
							stage[y][x + 1] == type;
							stage[y+1][x] == type;
							stage[y+1][x + 1] == type;

							if (PutBlock(y, x + 1, endY, endX, block, stage))
							{
								return true;
							}

							memcpy(stage, stageTemp, 4 * 22 * 22);
						}
					}

					block[kind][type]++;
				}
			}
		}
	}
#endif
	return false;
}

void slmp(const int block[4], int stage[22][22])
{
	int myBlock[2][5] = { 0 };
	int endY = 0;
	int endX = 0;
	int* pBlock = &stage[0][0];
	::memcpy(&myBlock[0][1],block,16);
	for (int y = 0; y < 22; y++)
	{
		for (int x = 0; x < 22; x++)
		{
			
			if (*pBlock>0)
			{
				if (endX<x)
				{
					endX = x;
				}
				if (endY<y)
				{
					endY = y;
				}
				if ((x==0||(*(pBlock-1) + *pBlock != 0 && (y==0||*(pBlock - 23) + *pBlock != 0))) && (y==0 || (*(pBlock - 22) + *pBlock != 0 && *(pBlock - 21) + *pBlock != 0)))
				{
					myBlock[0][*pBlock]--;
					myBlock[1][*pBlock]++;
				}
				(*pBlock) *= -1;
			}
			pBlock++;
		}
	}
	if (endX==0)
	{
		endX = 9;
	}
	printf("PutStage>>>RANGE{Y[%d],X[%d]} FREE{R[%d]G[%d]B[%d]Y[%d]} RESERVE{R[%d]G[%d]B[%d]Y[%d]} \n",endY,endX, myBlock[0][1], myBlock[0][2], myBlock[0][3], myBlock[0][4], myBlock[1][1], myBlock[1][2], myBlock[1][3], myBlock[1][4]);
	PrintStage(stage);

	PutBlock(0, 0, endY, endX, myBlock, stage);
}
void main()
{
	const int block[4] = {7,7,7,7};
	int stage[22][22] = {
		{ 2, 4, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 4, 4, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 3, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
	};

	printf("Input>>>R[%d]G[%d]B[%d]Y[%d]\n",block[0],block[1], block[2], block[3]);
	PrintStage(stage);

	slmp(block, stage);

	printf("Output>>>count[%d]\n", count);
	PrintStage(stage);

}