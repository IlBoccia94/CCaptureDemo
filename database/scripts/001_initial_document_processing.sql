CREATE TABLE IF NOT EXISTS documents (
    id uuid PRIMARY KEY,
    file_name varchar(255) NOT NULL,
    storage_path varchar(1000) NOT NULL,
    file_type integer NOT NULL,
    status integer NOT NULL,
    error_message varchar(4000),
    processing_started_at_utc timestamp with time zone,
    processing_completed_at_utc timestamp with time zone,
    created_by varchar(256),
    updated_by varchar(256),
    created_at_utc timestamp with time zone NOT NULL,
    updated_at_utc timestamp with time zone
);

CREATE TABLE IF NOT EXISTS document_images (
    id uuid PRIMARY KEY,
    document_id uuid NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    source_path varchar(1000) NOT NULL,
    page_number integer NOT NULL,
    label integer,
    detection_score numeric(10,6),
    cropped_path varchar(1000),
    overlay_path varchar(1000),
    created_at_utc timestamp with time zone NOT NULL,
    updated_at_utc timestamp with time zone
);

CREATE TABLE IF NOT EXISTS extracted_metadata (
    id uuid PRIMARY KEY,
    document_id uuid NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    document_image_id uuid NOT NULL REFERENCES document_images(id) ON DELETE CASCADE,
    field_name varchar(128) NOT NULL,
    field_value varchar(4000),
    confidence numeric(10,6),
    bounding_box_x integer,
    bounding_box_y integer,
    bounding_box_width integer,
    bounding_box_height integer,
    created_at_utc timestamp with time zone NOT NULL,
    updated_at_utc timestamp with time zone
);

CREATE INDEX IF NOT EXISTS ix_extracted_metadata_document_field
    ON extracted_metadata(document_id, field_name);

CREATE TABLE IF NOT EXISTS processing_logs (
    id uuid PRIMARY KEY,
    document_id uuid NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    step varchar(128) NOT NULL,
    message varchar(4000) NOT NULL,
    is_error boolean NOT NULL,
    created_at_utc timestamp with time zone NOT NULL,
    updated_at_utc timestamp with time zone
);

CREATE INDEX IF NOT EXISTS ix_processing_logs_document_created
    ON processing_logs(document_id, created_at_utc);
