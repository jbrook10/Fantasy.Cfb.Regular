export type YearType = 'Regular';
export type PositionType = 'qb' | 'wr' | 'rb' | 'te';

export interface ILeagueData {
    Year: number;
    SeasonType: YearType;
    LastUpdated: string;
    Weeks: IWeek[];
    Owners: IOwner[];
}

export interface IWeek {
    Number: number;
    Start: Date;
    End: Date;
}

export interface IOwner {
    Name: string;
    Players: IPlayer[];
    Weeks: IOwnerWeek[];

    // client side only
    TotalScore: number;
}

export interface IOwnerWeek {
    Number: number;
    Score: number;
    // Client Side Only
    PlayerCount: number;
}

export interface IPlayer {
    FantasyOwner: string;
    Name: string;
    Position: PositionType;
    Link: string;
    Logs: IGameLog[];
    School: string;
    Score: number;
    Avg: number;

    WorkingLogs: IGameLog[];
}

export interface IGameLog {
    Week: number;
    Date: Date;
    PassYds: number;
    RecYds: number;
    RushYds: number;
    Tds: number;
    Rec: number;
    Score: number;
    Opp: string;
    Vs: string;
    IsLog: boolean;
}

export interface IWeekScoreLog {
    Name: string;
    Link: string;
    Position: PositionType;
    PassYds: number;
    RecYds: number;
    RushYds: number;
    Tds: number;
    Rec: number;
    Score: number;
    Opponent: string;
    IsDescription: boolean;
    School: string;
    Date: Date;
    Vs: string;
    Complete: boolean;
}
