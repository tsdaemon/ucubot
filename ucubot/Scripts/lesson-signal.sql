CREATE TABLE lesson_signal (
     id BIGINT NOT NULL,
     timestamp_ TIMESTAMP,
     signal_type INT NOT NULL,
     user_id TIN NOT NULL,
     PRIMARY KEY (id)
);