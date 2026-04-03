export interface Payment {
  Id: number;
  Amount: string;
  Currency: number;
  Reference: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  IsDeleted: boolean;
}