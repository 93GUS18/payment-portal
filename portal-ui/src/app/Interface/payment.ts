export interface Payment {
  id: number;
  amount: string;
  currency: number;
  reference: string;
  createdAt: Date;
  updatedAt: Date;
  isDeleted: boolean;
}