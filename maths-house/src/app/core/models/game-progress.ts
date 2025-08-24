import { FloorId, FloorStatus } from './enums';

export interface GameProgress {
  version: number;
  floors: Record<FloorId, FloorStatus>;
  hasReachedPart2: boolean;
  collectedIngredients: string[];
  collectedMagicNumbers: Record<string, number>;
}
